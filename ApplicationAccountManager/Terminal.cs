using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoldenSIM.ApplicationAccouns
{
    /// <summary>
    /// Сводное описание для Terminal
    /// </summary>
    public class Terminal:ApplicationAccount
    {
        public Terminal()
        {
            //
            // TODO: добавьте логику конструктора
            //
        }

        public override string ButtonEdit
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string ButtonDelete
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override dynamic Profile
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal override void ParseProfile(dynamic profile)
        {
            throw new NotImplementedException();
        }
    }
}