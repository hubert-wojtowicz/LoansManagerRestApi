﻿using System;

namespace LoansManager.WebApi.Controllers.Models.LoansController
{
    public class GetLoanResponseModel
    {
        public Guid Id { get; set; }

        public decimal CommitmentValue { get; set; }

        public bool IsRepaid { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? RepaidDate { get; set; }

        public string LenderName { get; set; }

        public string BorrowerName { get; set; }
    }
}
