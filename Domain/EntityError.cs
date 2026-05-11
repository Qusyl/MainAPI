using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class EntityError
    {
        public string Message { get; set; }

        private EntityError(string message)
        {
            Message = message;
        }

        public static EntityError InvalidAmount => new EntityError("Не верный формат данных: amount");
        public static EntityError InvalidId => new EntityError("Не верный формат данных: Id");
        public static EntityError InvalidStatus => new EntityError("Не верный формат данных: status");
        public static EntityError InvalidCurrency => new EntityError("Не верный формат данных: currency");
        public static EntityError InvalidEmail => new EntityError("Не верный формат данных: email");
        public static EntityError InvalidPassword => new EntityError("Не верный формат данных: password");



    }
}
