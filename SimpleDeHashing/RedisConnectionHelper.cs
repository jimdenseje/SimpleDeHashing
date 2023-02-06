using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDeHashing
{

    public static class RedisConnectorHelper
    {
        public static void Connect(string IpOrUrl)
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(IpOrUrl);
            });
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection()
        {
            return lazyConnection.Value;
        }
    }

}
