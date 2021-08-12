using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class BankCashReceiptTypeModel : StandardEntity
    {
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string COACode { get; set; }
        public int COAId { get; set; }
        [StringLength(50)]
        public string COAName { get; set; }

    }
}
