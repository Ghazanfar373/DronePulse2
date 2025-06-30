using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using log4net;
using MissionPlanner.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

using MissionPlanner.Controls;
using MissionPlanner.ArduPilot.Mavlink; //.Controls;
using System.Runtime.Serialization.Formatters.Binary;

namespace MissionPlanner.Mavlink
{

    public class MAVMemAuthKeys
    {
        //::prot:: private static readonly ILog log =    LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //::prot::static string keyfile = Settings.GetUserDataDirectory() + "memAuthcurrUserRecord.xml";

        //::prot::static Crypto Rij = new Crypto();

        public static UserRecord currUserRecord = new UserRecord();

        //https://msdn.microsoft.com/en-us/library/aa347850(v=vs.110).aspx

        //[CollectionDataContract(ItemName = "UserRecord", Namespace = "")]
        [Serializable]
        public class UserRecord
        {
            public string name;
            public byte[] salt;

            public byte[] iv;

            public Dictionary<string, AuthKey> dict = new Dictionary<string, AuthKey>();
            public byte[] cipher;
            public byte[] tag;


            //public byte[] pbkdf_k1;
            //public byte[] pbkdf_k2;

        }


        [DataContract(Name = "AuthKey", Namespace = "")]
        [Serializable]
        public struct AuthKey
        {
            [DataMember()]
            public string UavName;
            [DataMember()]
            public byte[] MemKey;
        }

        static MAVMemAuthKeys()
        {
            //Rij.algorithm.Key = sa.Key; //::aes::
            //Rij.algorithm.IV = sa.IV;
            //::!::Load();
        }

        public static void AddKey(string name, string seed)
        {
            // sha the user input string
            SHA256Managed signit = new SHA256Managed();
            var shauser = signit.ComputeHash(Encoding.UTF8.GetBytes(seed));
            Array.Resize(ref shauser, 32);

            currUserRecord.dict[name] = new AuthKey() { MemKey = shauser, UavName = name };
        }


        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }


        // Convert a byte array to an Object
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }


        public static byte[] Encrypt(byte[] key, byte[] iv, byte[] data)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, encryptor);
                }
            }
        }

        public static byte[] Decrypt(byte[] key, byte[] iv, byte[] data)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, decryptor);
                }
            }
        }

        private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }

        /*
        //static SymmetricAlgorithm sa = SymmetricAlgorithm.Create(); //::aes:: I used this to provide key, IV at start
        public static void Save()
        {

            //byte[] iv_128bit = currUserRecord.iv.Take(16).ToArray();
            // update user keys data to be stored + calc cipher + tag
            currUserRecord.cipher = Encrypt(Login.User_pbkdf_k1, currUserRecord.iv, ObjectToByteArray( currUserRecord.dict)); //convert obj to plain bytes then encrypt
            currUserRecord.tag = getTag(currUserRecord.cipher, Login.User_pbkdf_k2);
            var tempHolder = currUserRecord.dict;
            currUserRecord.dict = null; //to not be stored as plain ::fix:: do it cleaner


            // save config
            DataContractSerializer writer =
                new DataContractSerializer(typeof(UserRecord));//,
                   // new Type[] { typeof(AuthKey) });

            //sa = SymmetricAlgorithm.Create();




            //TripleDES encAlg = TripleDES.Create();
            //using (var sw = new CryptoStream(fs, encAlg.CreateEncryptor(), CryptoStreamMode.Write))
            //::prot::using (var sw = new CryptoStream(fs, Rij.algorithm.CreateEncryptor(Rij.algorithm.Key, Rij.algorithm.IV), CryptoStreamMode.Write))

            using (var fs = new FileStream(keyfile, FileMode.Create))
           // using (Rijndael rijAlg = Rijndael.Create())
            {
                //::prot::writer.WriteObject(sw, Keys);
                writer.WriteObject(fs, currUserRecord);
            }

            currUserRecord.dict = tempHolder;
        }

        //static SymmetricAlgorithm sa = SymmetricAlgorithm.Create(); //::aes:: I used this to provide key, IV at start
        public static void SaveEnc()
        {
            // save config
            DataContractSerializer writer =
                new DataContractSerializer(typeof(UserRecord),
                    new Type[] { typeof(AuthKey) });

            //sa = SymmetricAlgorithm.Create();

            


                //TripleDES encAlg = TripleDES.Create();
            //using (var sw = new CryptoStream(fs, encAlg.CreateEncryptor(), CryptoStreamMode.Write))
            //::prot::using (var sw = new CryptoStream(fs, Rij.algorithm.CreateEncryptor(Rij.algorithm.Key, Rij.algorithm.IV), CryptoStreamMode.Write))

            using (var fs = new FileStream(keyfile, FileMode.Create))
            using (Rijndael rijAlg = Rijndael.Create())
            {

                SHA256Managed signit = new SHA256Managed();
                var userBytes = signit.ComputeHash(Encoding.UTF8.GetBytes(Login.Username));
                Array.Resize(ref userBytes, 32);


                var passHashBytes = signit.ComputeHash(Login.UserpasswordHash);
                //var passHashBytes = Login.UserpasswordHash;
                 Array.Resize(ref passHashBytes, 16);

                rijAlg.Key = userBytes;//::fix:: ::TODO:: pass proper Key 
                rijAlg.IV = passHashBytes; //::fix:: ::TODO:: pass proper IV (rando?)
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (var sr = new CryptoStream(fs, encryptor, CryptoStreamMode.Write))
                    {
                        //Write all data to the stream
                        writer.WriteObject(sr, currUserRecord);
                        // swEncrypt.Write(keys);
                        //encrypted = msEncrypt.ToArray();
                    }
                }

                //::prot::writer.WriteObject(sw, Keys);
                //writer.WriteObject(sw, Keys);
            }
        }


        public static void Load()
        {
            if (!File.Exists(keyfile))
                return;

            try
            {
                

                DataContractSerializer reader =
                    new DataContractSerializer(typeof(UserRecord));//,
                       // new Type[] { typeof(AuthKey) });



                //TripleDES encAlg = TripleDES.Create();
                using (var fs = new FileStream(keyfile, FileMode.Open))
                //using (Rijndael rijAlg = Rijndael.Create())
                {


                    //::prot::Keys = (AuthKeys)reader.ReadObject(sr);
                    currUserRecord = (UserRecord)reader.ReadObject(fs);
                    //::prot::writer.WriteObject(sw, Keys);
                    //writer.WriteObject(sw, Keys);
                }
                //byte[] iv_128bit = currUserRecord.iv.Take(16).ToArray();
                // update user keys data to be stored + calc cipher + tag
                byte[] dictBytes = Decrypt(Login.User_pbkdf_k1, currUserRecord.iv, currUserRecord.cipher); //convert obj to plain bytes then encrypt
                currUserRecord.dict = (Dictionary<string, AuthKey>)ByteArrayToObject(dictBytes);
                if (currUserRecord.dict == null)
                {  //if try to decrypt and failed (e.g signup with same name diff pass will make decryption fail (null)
                    currUserRecord.dict = new Dictionary<string, AuthKey>();
                }
                currUserRecord.tag = getTag(currUserRecord.cipher, Login.User_pbkdf_k2);
                //currUserRecord.cipher = null; //to not be stored as plain ::fix:: do it cleaner
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //::prot::log.Error(ex);
            }
        }
        //internal static void Load() //::aes:: I made this public to force call it from UI since not called
        public static void LoadEnc()
        {
            if (!File.Exists(keyfile))
                return;

            try
            {

                DataContractSerializer reader =
                    new DataContractSerializer(typeof(UserRecord),
                        new Type[] { typeof(AuthKey) });



                TripleDES encAlg = TripleDES.Create();
                using (var fs = new FileStream(keyfile, FileMode.Open))
                using (Rijndael rijAlg = Rijndael.Create())
                {

                    SHA256Managed signit = new SHA256Managed();
                    var userBytes = signit.ComputeHash(Encoding.UTF8.GetBytes(Login.Username));
                    Array.Resize(ref userBytes, 32);


                    var passHashBytes = signit.ComputeHash(Login.UserpasswordHash);
                    //var passHashBytes = Login.UserpasswordHash;
                    Array.Resize(ref passHashBytes, 16);


                    rijAlg.Key = userBytes;//::fix:: ::TODO:: pass proper Key 
                    rijAlg.IV = passHashBytes; //::fix:: ::TODO:: pass proper IV (rando?)
                    ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (var sr = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
                        {

                            //::prot::Keys = (AuthKeys)reader.ReadObject(sr);
                            currUserRecord = (UserRecord)reader.ReadObject(sr);
                        }
                    }

                    //::prot::writer.WriteObject(sw, Keys);
                    //writer.WriteObject(sw, Keys);
                }
                
            }
            catch (Exception ex)
            {
                //::prot::log.Error(ex);
            }
        }
        */
    }


}
