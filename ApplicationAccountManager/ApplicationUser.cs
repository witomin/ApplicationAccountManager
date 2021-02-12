using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GoldenSIM.ApplicationAccouns
{
    /// <summary>
    /// Класс пользователя
    /// </summary>
    public class ApplicationUser : ApplicationAccount
    {
        public override dynamic Profile
        {
            get
            {
                var result = new ApplicationUserProfile
                {
                    Name = Name,
                    Code = Code,
                    Address = Address,
                    IdManager = IdManager,
                    Contact = new ApplicationAccountContact
                    {
                        FullName = FullName,
                        Phone = Phone
                    },
                    IPaddreses = IPaddreses
                };
                return result;
            }
        }
        public override string ButtonEdit
        {
            get
            {
                if (IsApproved == 0) return string.Empty;
                return @"<div style = ""cursor:pointer;  float: left; margin: 5px;""class='ui-state-default ui-corner-all ui-icon ui-icon-pencil' title = 'Изменить' onclick='return updateUser($(this).parent(`td`).parent(`tr`));'></div>";
            }
        }

        public override string ButtonDelete
        {
            get
            {

                if (IsApproved == 1)
                    return string.Concat("<div style='cursor:pointer; float: left; margin: 5px;' class='ui-state-default ui-corner-all ui-icon ui-icon-close' title = 'Удалить' onclick='if (confirm(\"Вы действительно хотите удалить пользователя ", Login, "?\")){deleteUser(", Id, ");}'></div>");
                else
                    return string.Concat("<div style='cursor:pointer; float: left; margin: 5px;' class='ui-state-default ui-corner-all ui-icon ui-icon-arrowrefresh-1-n' title = 'Восстановить' onclick='if (confirm(\"Вы действительно хотите восстановить пользователя ", Login, "?\")){UnDeleteUser(", Id, ");}'></div>");

            }
        }

        public ApplicationUser()
        {
            IPaddreses = new List<string>();
        }
        public ApplicationUser(string Id, string Profile)
        {
            this.Id = Convert.ToInt32(Id);
            var newProfile = JsonConvert.DeserializeObject<ApplicationUserProfile>(Profile);
            ParseProfile(newProfile);

        }
        public ApplicationUser(string Profile)
        {
            var newProfile = JsonConvert.DeserializeObject<ApplicationUserProfile>(Profile);
            ParseProfile(newProfile);

        }
        public ApplicationUser(ApplicationUserProfile Profile)
        {
            ParseProfile(Profile);

        }
        internal override void ParseProfile(dynamic Profile)
        {
            if (Profile is ApplicationUserProfile)
            {
                Name = Profile.Name;
                Code = Profile.Code;
                Address = Profile.Address;
                IdManager = Profile.IdManager;
                FullName = Profile.Contact.FullName;
                Phone = Profile.Contact.Phone;
                IPaddreses = Profile.IPaddreses;
            }
        }
    }

}