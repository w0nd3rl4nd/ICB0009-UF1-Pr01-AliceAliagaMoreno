using System;
using System.Text;
using System.Security.Cryptography;
using ClaveSimetricaClass;
using ClaveAsimetricaClass;

namespace SimuladorEnvioRecepcion
{
    class Program
    {   
        static string UserName;
        static string SecurePass;  
        static ClaveAsimetrica Emisor = new ClaveAsimetrica();
        static ClaveAsimetrica Receptor = new ClaveAsimetrica();
        static ClaveSimetrica ClaveSimetricaEmisor = new ClaveSimetrica();
        static ClaveSimetrica ClaveSimetricaReceptor = new ClaveSimetrica();

        static byte[] Salt;

        static string TextoAEnviar = "Me he dado cuenta que incluso las personas que dicen que todo está predestinado y que no podemos hacer nada para cambiar nuestro destino igual miran antes de cruzar la calle. Stephen Hawking.";
        
        static void Main(string[] args)
        {

            /****PARTE 1****/
            //Login / Registro
            Console.WriteLine ("¿Deseas registrarte? (S/N)");
            string registro = Console.ReadLine ();

            if (registro =="S")
            {
                //Realizar registro del cliente
                Registro();                
            }

            //Realizar login
            bool login = Login();

            /***FIN PARTE 1***/

            if (login)
            {                  
                byte[] TextoAEnviar_Bytes = Encoding.UTF8.GetBytes(TextoAEnviar); 
                Console.WriteLine("Texto a enviar bytes: {0}", BytesToStringHex(TextoAEnviar_Bytes));    
                
                //LADO EMISOR

                //Firmar mensaje


                //Cifrar mensaje con la clave simétrica


                //Cifrar clave simétrica con la clave pública del receptor

                //LADO RECEPTOR

                //Descifrar clave simétrica

                
                //Descifrar clave simétrica
 

                //Descifrar mensaje con la clave simétrica


                //Comprobar firma

            }
        }

        public static void Registro()
        {
            Console.WriteLine ("Indica tu nombre de usuario:");
            UserName = Console.ReadLine();

            Console.WriteLine ("Indica tu password:");
            string passwordRegister = Console.ReadLine();

            // Generar salt aleatorio
            Salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(Salt);
            }

            // Crear hash con SHA512 + salt
            using (var sha512 = SHA512.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordRegister);
                byte[] passwordConSalt = new byte[Salt.Length + passwordBytes.Length];

                // Concatenar salt + password
                Buffer.BlockCopy(Salt, 0, passwordConSalt, 0, Salt.Length);
                Buffer.BlockCopy(passwordBytes, 0, passwordConSalt, Salt.Length, passwordBytes.Length);

                byte[] hashBytes = sha512.ComputeHash(passwordConSalt);
                SecurePass = Convert.ToBase64String(hashBytes); // Lo guardamos como string para comparar
            }

            Console.WriteLine("Registro completado con éxito.\n");
        }


        public static bool Login()
        {
            bool auxlogin = false;
            do
            {
                Console.WriteLine ("Acceso a la aplicación");
                Console.WriteLine ("Usuario: ");
                string userName = Console.ReadLine();

                Console.WriteLine ("Password: ");
                string Password = Console.ReadLine();

                if (userName == UserName)
                {
                    using (var sha512 = SHA512.Create())
                    {
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);
                        byte[] passwordConSalt = new byte[Salt.Length + passwordBytes.Length];

                        // Repetimos la misma concatenación que en el registro
                        Buffer.BlockCopy(Salt, 0, passwordConSalt, 0, Salt.Length);
                        Buffer.BlockCopy(passwordBytes, 0, passwordConSalt, Salt.Length, passwordBytes.Length);

                        byte[] hashBytes = sha512.ComputeHash(passwordConSalt);
                        string hashBase64 = Convert.ToBase64String(hashBytes);

                        if (hashBase64 == SecurePass)
                        {
                            Console.WriteLine("Login correcto.\n");
                            auxlogin = true;
                        }
                        else
                        {
                            Console.WriteLine("Contraseña incorrecta.\n");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Usuario incorrecto.\n");
                }

            } while (!auxlogin);

            return auxlogin;
        }

        static string BytesToStringHex (byte[] result)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in result)
                stringBuilder.AppendFormat("{0:x2}", b);

            return stringBuilder.ToString();
        }        
    }
}
