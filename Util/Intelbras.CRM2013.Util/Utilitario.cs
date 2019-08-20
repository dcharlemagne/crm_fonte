using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Services.Protocols;

namespace Intelbras.CRM2013.Util
{
    public class Utilitario
    {
        public static string TratarErro(Exception ex)
        {
            if (ex is ArgumentException)
                return ((ArgumentException)ex).Message;

            string messageError = SDKore.Helper.Error.GetMessageError(ex);

            if (ex is SoapException)
            {
                GravaEventView(messageError, EventLogEntryType.Warning);
                return ((SoapException)ex).Detail.InnerText;
            }

            GravaEventView(messageError, EventLogEntryType.Error);

            return "Erro inesperado.";
        }

        public static void GravaEventView(string message, EventLogEntryType type)
        {
            try
            {
                EventLog.WriteEntry("CRM 2013", (message.Length > 32000 ? message.Substring(0, 32000) : message), type, 5554);
            }
            catch { }
        }

        //public static string ObterMensagemErro(Exception ex, bool gravarLog = true)
        //{
        //    return string.Empty;
        //}

        public static T ConverterEnum<T>(string valor)
        {
            return (T)System.Enum.Parse(typeof(T), valor, true);
        }

        /// <summary>
        /// Converte o vlor Epoch em data
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ConverterEmData(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.ToLocalTime().AddSeconds(timestamp).AddHours(-1);
        }

        /// <summary>
        /// Converte uma data em formato epoch
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static double ConverterEmFormatoEpoch(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        #region Criptografia

        #region Variáveis Privadas
        public static AesManaged _symmetricAlgorithm;
        #endregion

        #region Propriedades Privadas
        /// <summary>
        /// Recuperar o tamanho do vetor de inicialização do algoritmo.
        /// </summary>
        private static int IVLength
        {
            get { return _symmetricAlgorithm.IV.Length; }
        }
        #endregion

        #region Métodos Privados Estáticos
        /// <summary>
        /// Transformar o dado conforme a operação (encriptação/descriptação).
        /// </summary>
        /// <param name="transform">Objeto responsável pela transformação da informação.</param>
        /// <param name="buffer">Conteúdo a ser transformado.</param>
        /// <returns>Resultado da transformação.</returns>
        /// <exception cref="CryptographicException">Erro ao transformar o dado conforme a operação (encriptação/decriptação).</exception>
        private static byte[] Transform(ICryptoTransform transform, byte[] buffer)
        {
            byte[] transformBuffer = null;

            using (var memoryStream = new MemoryStream())
            {
                CryptoStream cryptoStream = null;
                try
                {
                    cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
                    cryptoStream.Write(buffer, 0, buffer.Length);
                    cryptoStream.FlushFinalBlock();
                    transformBuffer = memoryStream.ToArray();
                }
                #region Tratamento de Erros
                catch (Exception ex)
                {
                    throw new CryptographicException("Erro ao transformar o dado conforme a operação (encriptação/decriptação).", ex);
                }
                finally
                {
                    if (cryptoStream != null)
                    {
                        cryptoStream.Close();
                        ((IDisposable)cryptoStream).Dispose();
                    }
                }
                #endregion
            }

            return transformBuffer;
        }

        /// <summary>
        /// Extrair o 'IV' do conteúdo encriptado.
        /// </summary>
        /// <param name="encryptedText">Conteúdo encriptado.</param>
        /// <returns>Conteúdo do 'IV'.</returns>
        /// <exception cref="CryptographicException">Erro ao extrair o 'IV do conteúdo encriptado.</exception>
        private static byte[] ExtractIV(byte[] encryptedText)
        {

            var initialVector = new byte[IVLength];

            if (encryptedText.Length < (IVLength + 1))
            {
                throw new CryptographicException("Erro ao extrair o 'IV' do conteúdo encriptado.");
            }

            var data = new byte[encryptedText.Length - IVLength];


            Buffer.BlockCopy(encryptedText, 0, initialVector, 0, IVLength);
            Buffer.BlockCopy(encryptedText, IVLength, data, 0, data.Length);

            _symmetricAlgorithm.IV = initialVector;


            return data;
        }

        /// <summary>
        /// Inicializar a chave do algoritmo simétrico.
        /// </summary>
        /// <param name="key">Conteúdo da chave.</param>
        /// <exception cref="CryptographicException">O tamanho da chave do algoritmo simétrico selecionado é inválido.</exception>
        private static void InitializeKey(byte[] key)
        {

            if (_symmetricAlgorithm.Key.Length != key.Length)
            {
                throw new CryptographicException("O tamanho da chave do algoritmo simétrico selecionado é inválido.");
            }

            _symmetricAlgorithm.Key = key;

        }

        /// <summary>
        /// Encriptar um determinado conteúdo.
        /// </summary>
        /// <param name="plainText">Conteúdo a ser encriptado.</param>
        /// <param name="key">Chave utilizada no processo de criptografia.</param>
        /// <returns>Resultado do processo de criptografia.</returns>
        private static byte[] Encrypt(byte[] plainText, byte[] key)
        {
            byte[] output = null;
            byte[] cipherText = null;

            try
            {
                InitializeKey(key);
                using (var transform = _symmetricAlgorithm.CreateEncryptor())
                {
                    cipherText = Transform(transform, plainText);
                }

                output = new byte[IVLength + cipherText.Length];
                Buffer.BlockCopy(_symmetricAlgorithm.IV, 0, output, 0, IVLength);
                Buffer.BlockCopy(cipherText, 0, output, IVLength, cipherText.Length);
            }
            #region Tratamento de Erros
            catch (CryptographicException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Erro ao encriptar um determinado conteúdo.", ex);
            }
            finally
            {
                Array.Clear(_symmetricAlgorithm.Key, 0, _symmetricAlgorithm.Key.Length);
            }
            #endregion

            return output;
        }

        /// <summary>
        /// Decriptar um determinado conteúdo.
        /// </summary>        
        /// <param name="cipherText">Conteúdo encriptado.</param>
        /// <param name="key">Chave utilizada no processo de criptografia.</param>
        /// <returns>Conteúdo em claro.</returns>
        private static byte[] Decrypt(byte[] cipherText, byte[] key)
        {
            byte[] output = null;
            byte[] data = null;

            try
            {
                InitializeKey(key);
                data = ExtractIV(cipherText);

                using (var transform = _symmetricAlgorithm.CreateDecryptor())
                {
                    output = Transform(transform, data);
                }
            }
            #region Tratamento de Erros
            catch (CryptographicException)
            {

                throw;
            }
            catch (Exception ex)
            {

                throw new CryptographicException("Erro ao decriptar um determinado conteúdo.", ex);
            }
            finally
            {
                Array.Clear(_symmetricAlgorithm.Key, 0, _symmetricAlgorithm.Key.Length);
            }
            #endregion


            return output;
        }

        /// <summary>
        /// Encriptar um determinado conteúdo.
        /// </summary>
        /// <param name="plainText">Conteúdo a ser encriptado no formato Base64.</param>
        /// <param name="key">Chave utilizada no processo de criptografia.</param>
        /// <returns>Resultado do processo de criptografia no formato Base64.</returns>
        public static string EncryptSymmetric(string plainText, byte[] key)
        {
            #region Validar parâmetros
            if (string.IsNullOrEmpty(plainText) == true)
            {
                throw new ArgumentNullException("plainText");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            #endregion

            return Convert.ToBase64String(Encrypt(Encoding.Unicode.GetBytes(plainText), key));
        }


        /// <summary>
        /// Decriptar um determinado conteúdo.
        /// </summary>
        /// <param name="ciphertextBase64">Conteúdo encriptado no formato Base64.</param>
        /// <param name="key">Chave utilizada no processo de criptografia.</param>
        /// <returns>Conteúdo em claro.</returns>
        public static string DecryptSymmetric(string ciphertextBase64, byte[] key)
        {

            #region Validar parâmetros
            if (string.IsNullOrEmpty(ciphertextBase64) == true)
            {

                throw new ArgumentNullException("ciphertextBase64");
            }




            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            #endregion



            var decryptedBytes = Decrypt(Convert.FromBase64String(ciphertextBase64), key);
            return Encoding.Unicode.GetString(decryptedBytes);

        }
        #endregion

        #endregion
    }
}
