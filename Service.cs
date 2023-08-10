using GrapeCity.Documents.Html;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace Booking
{

    public class Service
    {
        private string _Act = ""; //手機
        private string _PWS = ""; //密碼
        //private string _StoreId = "S2212290010";//響食天堂
        private string _StoreId = "S2212290004";//響響微風

        private string bookingDate;
        //lunch tea dinner
        private string bookingTime;
        private int bookingPeople;
        //有空位的時間
        private string enableTime;

        private int mealSeq;
        private string token;


        public Service(string date, string time, int people)
        {
            bookingDate = date;
            bookingTime = time;
            bookingPeople = people;
        }


        public string Login()
        {
            string url = @"https://www.feastogether.com.tw/api/994f5388-d001-4ca4-a7b1-72750d4211cf/custSignIn";

            HttpClient client = new HttpClient() { };

            var postData = new { act = _Act, pwd = _PWS, countryCode = "TW", iCode = "+886", memberAccessToken = "" };

            string json = JsonConvert.SerializeObject(postData);

            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(url, contentPost).GetAwaiter().GetResult();

            var result = JsonConvert.DeserializeObject<Login>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            if (result == null)
            {
                return string.Empty;
            }
            if (result.statusCode == 1000)
            {
                Console.WriteLine($"Tooken: {result.result.customerLoginResp.token}");
                token = string.Format("Bearer {0}", result.result.customerLoginResp.token);
                //正常
                return token;
            }
            else
            {
                //異常
                return string.Empty;
            }
        }

        public SVG GetSVG()
        {
            string url = @"https://www.feastogether.com.tw/api/994f5388-d001-4ca4-a7b1-72750d4211cf/get2FASvgByBrand";

            HttpClient client = new HttpClient() { };

            var postData = new { brandId = "BR00002" };


            string json = JsonConvert.SerializeObject(postData);

            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(url, contentPost).GetAwaiter().GetResult();

            var result = JsonConvert.DeserializeObject<CheckSVG>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            return new SVG()
            {
                html = result.result.svg,
                code = result.result.code
            };
        }

        public void RenderSVG(SVG svg)
        {
            var browserPath = BrowserFetcher.GetSystemEdgePath();
            using var browser = new GcHtmlBrowser(browserPath);

            //now set up the variable containing the "thank you for shopping" message

            string html = "<!DOCTYPE html>" +
            "<html>" +
            "<head>" +
            "<style>" +
            "</style>" +
            "</head>" +
            "<body>" +
            svg.html +
            "</body>" +
            "</html>";

            //now create a browser instance and load the HTML string
            using var pg = browser.NewPage(html, new PageOptions
            {
                DefaultBackgroundColor = Color.White,
                WindowSize = new Size(150, 80)
            });

            //now create the png file
            pg.SaveAsPng("Validate.png");

            var process = new Process();

            process.StartInfo = new ProcessStartInfo(@"Validate.png")
            {
                UseShellExecute = true
            };

            process.Start();

        }


        public int searchBookingAble()
        {
            string url = @"https://www.feastogether.com.tw/api/booking/searchBookingAble";

            HttpClient client = new HttpClient() { };

            client.DefaultRequestHeaders.Add("act", _Act);
            client.DefaultRequestHeaders.Add("authorization", token);

            var postData = new
            {
                adult = bookingPeople
                ,
                child = 0
                ,
                mealDate = bookingDate
                ,
                mealPeriod = bookingTime
                ,
                storeId = _StoreId
            };

            string json = JsonConvert.SerializeObject(postData);

            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(url, contentPost).GetAwaiter().GetResult();

            var result = JsonConvert.DeserializeObject<Avable>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            if (result == null)
            {
                return 101010;
            }
            else
            {
                var target = result.result.targetTimes.Where(x => x.isEnable);

                //目標館有空位
                if (target.Count() > 0)
                {
                    enableTime = target.FirstOrDefault().time;
                    mealSeq = target.FirstOrDefault().mealSeq;
                    return 105007;
                }
                //目標館無空位
                else
                {
                    Console.WriteLine("已無閒置時間段");
                    return 101010;
                }
            }
        }


        public int SaveSeats(string verifyStr, string svgCode)
        {
            string url = @"https://www.feastogether.com.tw/api/booking/saveSeats";

            var postData = new
            {
                mealDate = bookingDate,
                mealPeriod = bookingTime,
                mealSeq = mealSeq,
                mealTime = enableTime,
                peopleCount = bookingPeople,
                scgVerifyStr = verifyStr ?? string.Empty,
                storeId = _StoreId,
                zked = "1j6ul4y94ejru6xk7vu4vu4",
                svgCode = svgCode
            };

            HttpClient client = new HttpClient() { };

            //手機
            client.DefaultRequestHeaders.Add("act", _Act);
            client.DefaultRequestHeaders.Add("authorization", token);

            string json = JsonConvert.SerializeObject(postData);

            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine("送出保留座位REQEUST");

            HttpResponseMessage response = client.PostAsync(url, contentPost).GetAwaiter().GetResult();

            var result = JsonConvert.DeserializeObject<SaveSets>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            Console.WriteLine($"保留座位 Status: {result.message} CODE: {result.statusCode}");

            return result.statusCode;
        }

        public void SendBooking()
        {
            string url = @"https://www.feastogether.com.tw/api/booking/booking";

            HttpClient client = new HttpClient() { };

            //手機
            client.DefaultRequestHeaders.Add("act", _Act);
            client.DefaultRequestHeaders.Add("authorization", token);

            List<ChargeList> chargeLists = new List<ChargeList>();

            chargeLists.Add(new ChargeList()
            {
                seq = 201,
                count = bookingPeople
            });

            chargeLists.Add(new ChargeList()
            {
                seq = 202,
                count = 0
            });

            var postData = new
            {
                adult = bookingPeople
                ,
                chargeList = chargeLists.ToArray()
                ,
                storeId = _StoreId
                ,
                child = 0
                ,
                childSeat = 0
                ,
                domain = "https://www.feastogether.com.tw"
                ,
                mealDate = bookingDate
                ,
                mealPeriod = bookingTime
                ,
                mealPurpose = ""
                ,
                mealSeq = mealSeq
                ,
                mealTime = enableTime
                ,
                pathFir = "booking"
                ,
                pathSec = "result"
                ,
                redirectType = "iEat_card"
                ,
                special = 0
                ,
                //storeCode = "NTBQ"
                storeCode = "IPDS"
                ,
                yuuu = "892389djdj883831445"
            };

            string json = JsonConvert.SerializeObject(postData);

            int statusCode = 101007;

            while (statusCode == 101007)
            {
                HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(url, contentPost).GetAwaiter().GetResult();

                //var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                Console.WriteLine(string.Format("Bookin Status: {0}", response.StatusCode));


                if (response.StatusCode == HttpStatusCode.OK)
                {
                    statusCode = 200;
                    Console.WriteLine("訂位成功!!!!");
                }

                if (statusCode == 101007)
                {
                    Console.WriteLine("訂位失敗繼續嘗試");
                    Thread.Sleep(2000);                    
                }
            }
        }


        public void SendMail(string date)
        {
            try
            {
                //設定smtp主機
                string smtpServer = "smtp.gmail.com";
                //設定Port
                //int portNumber = 587;              
                //填入寄送方email和密碼
                string mailFrom = "swatduck@gmail.com";
                string password = "vnsrdintjdqqzrly";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(mailFrom);
                    mail.To.Add("swatduck@gmail.com");

                    mail.Subject = "饗饗空位通知信";
                    mail.Body = $"{date}已有空位，請快速訂位";

                    // 若你的內容是HTML格式，則為True
                    mail.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient(smtpServer))
                    {
                        smtp.Credentials = new NetworkCredential(mailFrom, password);
                        smtp.Port = 25;
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
