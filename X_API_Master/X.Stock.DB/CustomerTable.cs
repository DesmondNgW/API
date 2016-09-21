using MongoDB.Driver.Builders;
using X.Stock.Model;
using X.Util.Extend.Mongo;

namespace X.Stock.DB
{
    public class CustomerTable
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="customerNo"></param>
        /// <returns></returns>
        public static CustomerInfo GetCustomerInfo(string customerNo)
        {
            var query = Query.EQ("CustomerNo", customerNo);
            return MongoDbBase<CustomerInfo>.Default.FindOne("Stock", "Customer", null, query);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerNo"></param>
        /// <param name="customerName"></param>
        /// <param name="coinAsset"></param>
        public static void InitCustomerInfo(string customerNo, string customerName, double coinAsset)
        {
            var cus = GetCustomerInfo(customerNo);
            if (cus == null)
            {
                MongoDbBase<CustomerInfo>.Default.InsertMongo(new CustomerInfo
                {
                    CustomerName = customerName,
                    CustomerNo = customerNo,
                    CoinAsset = coinAsset,
                    Id = customerNo
                }, "Stock", "Customer", null);
            }
        }

        /// <summary>
        /// 更新用户余额
        /// </summary>
        /// <param name="customerNo"></param>
        /// <param name="coinAsset">变化值</param>
        public static void UpdateCustomerInfo(string customerNo, double coinAsset)
        {
            var query = Query.EQ("CustomerNo", customerNo);
            var update = Update.Inc("CoinAsset", coinAsset);
            MongoDbBase<CustomerInfo>.Default.UpdateMongo("Stock", "Customer", null, query, update);
        }
    }
}
