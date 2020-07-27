using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace Aio
{
    public class LinkHelper
    {
        private static readonly DateTime DateStart = new DateTime(1970, 1, 1);

        public static double CurrentTimeMillis()
        {
            return (DateTime.UtcNow - DateStart).TotalMilliseconds;
        }
        
        public static byte[] GenKeyExchangeNonceAndSetInOutSecurity(Link link, byte[] userName, byte[] token,
            byte[] nonce)
        {
            var password = MD5.Create().ComputeHash(Concat(userName, token));
            var username = userName;

            var outkey = GenerateKeyByPassword(username, password, nonce);

            link.OutputSecurity = outkey;

            var random = new byte[16];
            new Random(DateTime.Now.Millisecond).NextBytes(random);
            var copy = new Octets(random);
            var inkey = GenerateKeyByPassword(username, password, random);
            link.InputSecurity = inkey;

            return copy.GetBytes();
        }
        

        private static byte[] Concat(byte[] a, byte[] b)
        {
            var c = new byte[a.Length + b.Length];
            Array.Copy(a, c, a.Length);
            Array.Copy(b, 0, c, a.Length, b.Length);
            return c;
        }

        private static byte[] GenerateKeyByPassword(byte[] identity, byte[] password, byte[] nonce)
        {
            return new HMACMD5(password).ComputeHash(Octets.Wrap(identity).Append(nonce).GetBytes());
        }

        private static bool isCarrierDataNetworkInterface(NetworkInterface netWork)
        {
            return netWork.NetworkInterfaceType == NetworkInterfaceType.Ppp ||
                    netWork.NetworkInterfaceType == NetworkInterfaceType.Unknown;
                    // string.IsNullOrEmpty(netWork.Name) || netWork.Name[0] == '\0';
        }

        public static IPAddress[] GetCarrierDataHostIPAddresses(bool isAll)
        {
            List<NetworkInterface> carrierNics = new List<NetworkInterface>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var netWork in nics)
            {
                if (!isCarrierDataNetworkInterface(netWork))
                    continue;
                carrierNics.Add(netWork);
            }
            return _getLocalHostIPAddresses(carrierNics.ToArray(), isAll);
        }

        public static IPAddress[] GetLocalAreaHostIPAddresses(bool isAll)
        {
            List<NetworkInterface> localAreaNics = new List<NetworkInterface>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var netWork in nics)
            {
                if (isCarrierDataNetworkInterface(netWork))
                    continue;
                localAreaNics.Add(netWork);
            }
            return _getLocalHostIPAddresses(localAreaNics.ToArray(), isAll);
        }

        public static IPAddress[] GetLocalHostIPAddresses(bool isAll)
        {
            return _getLocalHostIPAddresses(NetworkInterface.GetAllNetworkInterfaces(), isAll);
        }

        private static IPAddress[] _getLocalHostIPAddresses(NetworkInterface[] nics, bool isAll)
        {
            List<IPAddress> allIP = new List<IPAddress>();
            foreach (var netWork in nics)
            {
                UnityEngine.Debug.Log("netinterface :" + netWork.NetworkInterfaceType + ", " + netWork.IsReceiveOnly + ", " + netWork.OperationalStatus + ", " + netWork.GetPhysicalAddress() + ", " + netWork.Name + ";");
                // 单个网卡的IP对象
                IPInterfaceProperties ipInterface = netWork.GetIPProperties();
                foreach (var ipInfo in ipInterface.UnicastAddresses)
                {
                    UnityEngine.Debug.Log("ipInfo :" + ipInfo.Address);
                    allIP.Add(ipInfo.Address);
                }

                if (isAll)
                {
                    foreach (var ipInfo in ipInterface.MulticastAddresses)
                    {
                        allIP.Add(ipInfo.Address);
                    }

                    foreach (var ipInfo in ipInterface.AnycastAddresses)
                    {
                        allIP.Add(ipInfo.Address);
                    }
                }
            }
            return allIP.ToArray();
        }

        public static string linkIp = "182.92.232.154";
        public static string linkIpv6 = "";

        public static int linkPort = 10016;

        public static string userNamePrefix = "prefix";
        public static string userNameOverride = "";
    }
}