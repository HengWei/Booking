namespace Booking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //搶位順序
            string[] time = new string[] { "lunch", "dinner", "tea" };

            int idxTime = 0;

            //正常都是搶一個月後的日期 除非月底會開放到最後一天 這時候使用指定
            string date = DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd");
            //string date = "2023-09-30";

            int people = 2;

            Console.WriteLine($"日期: {date} , 時間: {time[idxTime]}, 人數: {people}");
          
            //lunch tea dinner
            Service service = new Service(date, time[idxTime], people);            

            var tooken = service.Login();

            if(string.IsNullOrEmpty(tooken))
            {
                Console.WriteLine("Tooken取得異常");
                Console.ReadKey();
            }            

            Console.WriteLine("取得Token完畢，準備進行搶位"); 

            //先打開來等
            bool isTimeUp = false;

            var datetimeNow = DateTime.Now;

            DateTime startTime = new DateTime(datetimeNow.Year, datetimeNow.Month, datetimeNow.Day, 9, 0, 0);

            while (isTimeUp==false)
            {
                if (datetimeNow>= startTime)
                {
                    isTimeUp = true;
                }

                Task.Delay(1000).Wait();
            }

            //嘗試次數
            int searchTime = 0;

            //初始狀態
            int statusCode = 101010;

            //驗證字串
            string verifyStr = string.Empty;

            //驗證資訊
            SVG sVG = new SVG();

            while (true)
            {
                //if(idxTime>2)
                //{
                //    Console.WriteLine("時段皆已滿");
                //    break;
                //}

                //101010 : 客滿
                if (statusCode == 101010)
                {
                    Console.WriteLine($"第 {++searchTime} 次 {time[idxTime]} 搶位開始");

                    statusCode = service.searchBookingAble();
                }
                //105007 : 驗證碼錯誤
                else if (statusCode == 105007)
                {
                    sVG = service.GetSVG();
                    service.RenderSVG(sVG);

                    //空白驗證碼不送出
                    while (string.IsNullOrEmpty(verifyStr))
                    {
                        Console.Write("輸入驗證碼:");
                        verifyStr = Console.ReadLine();
                    }

                    statusCode = service.SaveSeats(verifyStr, sVG.code);
                }
                //訂訂成功
                else if(statusCode==1000)
                {
                    service.SendBooking();
                    break;
                }

                //101027 : 訂位人數擁擠中
                //105005 : 目前人潮壅擠中
                else
                {
                    statusCode = service.SaveSeats(verifyStr, sVG.code);
                }
            }
        }
    }
}