﻿namespace Booking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Service service = new Service("2023-09-05", "tea", 2);

            var tooken = service.Login();

            if(string.IsNullOrEmpty(tooken))
            {
                Console.WriteLine("Tooken取得異常");
                Console.ReadKey();
            }            

            Console.WriteLine("取得Token完畢，準備進行搶位");            


            bool isTimeUp = false;

            var datetimeNow = DateTime.Now;

            DateTime startTime = new DateTime(datetimeNow.Year, datetimeNow.Month, datetimeNow.Day, 9, 0, 0);

            while (isTimeUp==false)
            {
                if (datetimeNow>= startTime)
                {
                    isTimeUp = true;
                }

                Task.Delay(1000);
            }

            int searchTime = 0;

            bool isAvable = false;

            while (isAvable==false)
            {
                Console.WriteLine($"第 {++searchTime} 搶位開始" );
                isAvable = service.searchBookingAble();
            }

            //驗證碼
            //可能可以先提取並識別
            var result = service.GetSVG();

            service.RenderSVG(result);

            Console.WriteLine("輸入驗證碼:");
            var verifyStr = Console.ReadLine();


            service.SaveSeats(verifyStr??string.Empty, result.code);

            service.SendBooking();
        }
    }
}