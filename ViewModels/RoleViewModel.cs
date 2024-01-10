using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
    public class RoleViewModel
    {
        public string ID { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public RoleViewModel()
        {
            ID = Guid.NewGuid().ToString();
        }
    }
}
