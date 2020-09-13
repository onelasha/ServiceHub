using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class UserModel
    {
        public int UserId { get; set; }
        public int StaffId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string PIN { get; set; }
        public string Address { get; set; }
        public string BirthPlace { get; set; }
        public int CitizenshipId { get; set; }
        public string Citizenship { get; set; }
        public string PassportNom { get; set; }
        public string ContractNom { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public int StatusId { get; set; }
        public string StatusDescription { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentDescription { get; set; }
        public int PositionId { get; set; }
        public string PositionDescription { get; set; }
        public string Code { get; set; }
        public string Username { get; set; }
        public object IsMed { get; set; }
        public object IsSales { get; set; }
        public object IsBlocked { get; set; }
        public string Permissions { get; set; }
        public string Email { get; set; }
        public bool ResetOnly { get; set; }
        public bool Activate { get; set; }
        public string ActivationToken { get; set; }
        public string NewPassword { get; set; }
        public string RePassword { get; set; }
    }
}
