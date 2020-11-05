using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MerchantService.Models;

namespace MerchantService.Models
{
    public class mCardType
    {
        public string BINCode { get; set; }
        public string CardType { get; set; }

        public string get_card_type()
        {
            using (MerchantService db = new MerchantService())
            {
                t_CardBin bin = db.t_CardBin.Where(x => x.BINCode == BINCode).SingleOrDefault();
                if(bin!=null)
                {
                    BINCode = bin.BINCode;
                    CardType = bin.CardType;
                }
                else
                {
                    BINCode = BINCode;
                    CardType = "Unknown Card";
                }
            }

            return CardType;
        }

        public static implicit operator mCardType(string v)
        {
            throw new NotImplementedException();
        }
    }
    
}