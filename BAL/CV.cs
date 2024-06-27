namespace HealthifyConsume.BAL
{
    public class CV
    {
        private static IHttpContextAccessor _httpContextAccessor;

        static CV()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }

        public static int? UserID()
        {
            int? UserID = null;

            if (_httpContextAccessor.HttpContext.Session.GetInt32("UserID") != null)
            {
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.Session.GetInt32("UserID"));
            }
            return UserID;
        }
        public static string? FirstName()
        {
            string? FirstName = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("FirstName") != null)
            {
                FirstName = _httpContextAccessor.HttpContext.Session.GetString("FirstName").ToString();
            }
            return FirstName;
        }

        public static string? LastName()
        {
            string? LastName = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("LastName") != null)
            {
                LastName = _httpContextAccessor.HttpContext.Session.GetString("LastName").ToString();
            }
            return LastName;
        }
        public static string? UserName()
        {
            string? UserName = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("UserName") != null)
            {
                UserName = _httpContextAccessor.HttpContext.Session.GetString("UserName").ToString();
            }
            return UserName;
        }

        public static string? Email()
        {
            string? Email = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("Email") != null)
            {
                Email = _httpContextAccessor.HttpContext.Session.GetString("Email").ToString();
            }
            return Email;
        }
        //HttpContext.Session.SetInt32("IsAdmin", Convert.ToInt32(user.IsAdmin));
        public static bool? IsActive()
        {
            bool? ISActive = null;

            if (_httpContextAccessor.HttpContext.Session.GetInt32("IsActive") != null)
            {
                ISActive = Convert.ToBoolean(_httpContextAccessor.HttpContext.Session.GetInt32("IsActive"));
            }
            return ISActive;
        }

        public static bool? IsAdmin()
        {
            bool? ISActive = null;

            if (_httpContextAccessor.HttpContext.Session.GetInt32("IsAdmin") != null)
            {
                ISActive = Convert.ToBoolean(_httpContextAccessor.HttpContext.Session.GetInt32("IsAdmin"));
            }
            return ISActive;
        }



    }
}
