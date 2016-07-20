using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tasks.Service.Dto
{
    /// <summary>
    /// Enum extension class.
    /// Provides method to extend <see cref="System.Guid">System.Guid</see>
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets enum variable label name.
        /// </summary>
        /// <param name="item">Enum variable</param>
        /// <returns>Enum variable label name</returns>
        public static string KeyName(this Enum item)
        {
            return Enum.GetName(item.GetType(), item);
        }

        /// <summary>
        /// Determines whether enum variable is marked with Required attribute.
        /// </summary>
        /// <param name="item">Enum variable</param>
        /// <returns>True or false</returns>
        public static bool Required(this Enum item)
        {
            FieldInfo field = item.GetType().GetField(item.ToString());
            Attribute attribute = Attribute.GetCustomAttribute(field, typeof(RequiredAttribute));

            return !attribute.Null();
        }

        /// <summary>
        /// Gets the value of DefaultValue attribute which is marked with specified enum variable.
        /// </summary>
        /// <param name="item">Enum variable</param>
        /// <returns>The value of DefaultValue attribute</returns>
        public static string DefaultValue(this Enum item)
        {
            FieldInfo field = item.GetType().GetField(item.ToString());
            DefaultValueAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DefaultValueAttribute)) as DefaultValueAttribute;

            return attribute.Null() ? string.Empty : attribute.Value.ToString();
        }
    }
}
