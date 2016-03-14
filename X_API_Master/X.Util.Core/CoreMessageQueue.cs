using System;
using System.Reflection;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace X.Util.Core
{
    public class CoreMessageSender
    {
        //public static void Connect(int port)
        //{
        //    try
        //    {
        //        IConnectionFactory factory = new ConnectionFactory(string.Format("tcp://localhost:{0}/", port));
        //        using (IConnection connection = factory.CreateConnection())
        //        {
        //            using (ISession session = connection.CreateSession())
        //            {
        //                IMessageProducer prod = session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic("testing"));
        //                int i = 0;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, e.ToString);
        //    }
        //}
    }

    public class CoreMessageReceiver
    {

    }
}
