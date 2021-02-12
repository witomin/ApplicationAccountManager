using System.Collections.Generic;

namespace GoldenSIM.ApplicationAccouns
{
    /// <summary>
    /// Профиль пользователя
    /// </summary>
    public class ApplicationUserProfile : ApplicationAccountProfile
    {
        public ApplicationUserProfile()
        {
            Contact = new ApplicationAccountContact();
            IPaddreses = new List<string>();
        }
    }
}