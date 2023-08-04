using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking
{
    internal class Model
    {
    }


    public class BaseRoot
    {
        public int statusCode { get; set; }
        public DateTime timestamp { get; set; }
        public string message { get; set; }
    }
    public class CustomerLoginResp
    {
        public string memberShip { get; set; }
        public string city { get; set; }
        public string area { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string birthYear { get; set; }
        public string birthMonth { get; set; }
        public string birthDay { get; set; }
        public string address { get; set; }
    }



    public class LoginCheckResult
    {
        public string msg { get; set; }
    }

    public class LoginCheck
    {
        public int statusCode { get; set; }
        public DateTime timestamp { get; set; }
        public string path { get; set; }
        public LoginCheckResult result { get; set; }
    }


    public class LoginResult
    {
        public CustomerLoginResp customerLoginResp { get; set; }
        public string _ACT { get; set; }
        public string _P { get; set; }
    }

    public class Login : BaseRoot
    {
        public LoginResult result { get; set; }
    }

    public class ChargeList
    {
        public int seq { get; set; }

        public int count { get; set; }
    }


    public class BookingResult
    {
        public string bookingId { get; set; }
        public string brandName { get; set; }
        public string storeName { get; set; }
        public string mealDate { get; set; }
        public string mealTime { get; set; }
        public string mealPeriod { get; set; }
        public string bookingState { get; set; }
        public string paymentState { get; set; }
        public object expireTime { get; set; }
    }

    public class Booking : BaseRoot
    {
        public BookingResult result { get; set; }
    }

    public enum OrderTime
    {
        lunch,
        tea,
        dinner
    }



    public class SaveSetsResult
    {
        public string msg { get; set; }
    }

    public class SaveSets : BaseRoot
    {
        public string path { get; set; }
        public SaveSetsResult result { get; set; }
    }



    public class OtherStore
    {
        public string storeId { get; set; }
        public string storeName { get; set; }
        public int storeOrder { get; set; }
        public string pcImg { get; set; }
        public string mobileImg { get; set; }
        public List<Time> times { get; set; }
    }

    public class Recommendation
    {
        public string storeId { get; set; }
        public string storeName { get; set; }
        public string brandId { get; set; }
        public string brandEngName { get; set; }
        public string brandName { get; set; }
        public string brandLogo { get; set; }
    }

    public class AvableResult
    {
        public List<Recommendation> recommendation { get; set; }
        public List<OtherStore> otherStores { get; set; }
        public List<TargetTime> targetTimes { get; set; }
    }

    public class Avable : BaseRoot
    {
        public AvableResult result { get; set; }
    }

    public class TargetTime
    {
        public string time { get; set; }
        public bool isEnable { get; set; }
        public int mealSeq { get; set; }
    }

    public class Time
    {
        public string time { get; set; }
        public bool isEnable { get; set; }
        public int mealSeq { get; set; }
    }


    public class CheckSVGResult
    {
        public string code { get; set; }
        public string svg { get; set; }
        public bool fkd { get; set; }
    }

    public class CheckSVG : BaseRoot
    {
        public CheckSVGResult result { get; set; }
    }


    public class Result
    {
        public List<Situation> situation { get; set; }
    }

    public class AvableInfo
    {
        public int statusCode { get; set; }
        public DateTime timestamp { get; set; }
        public string message { get; set; }
        public Result result { get; set; }
    }

    public class Situation
    {
        public string date { get; set; }
        public string surplusText { get; set; }
        public bool isOffDay { get; set; }
        public bool isFull { get; set; }
    }

    public class  SVG
    {
        //GUID
        public string code { get; set; }

        public string html { get; set; }
    }
}
