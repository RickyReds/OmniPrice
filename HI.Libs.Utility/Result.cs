using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HI.Libs.Utility
{

    /*      PROTOTIPO TIPICO DI UTILIZZO:
     
            public Result Metodo()
            {
                Result r = new Result();

                try
                {
                    #region


                    #endregion

                    r.IsOk = true;
                }
                catch(Exception ex)
                {
                    r.IsOk = false;
                    r.Exception = ex;
                }

                return r;
            }


    */

    public class Result : Result<object>
    {
        public Result([CallerMemberName]string Reference = "") : base(Reference)
        {

        }
    }                                         //Versione derivata Result<object> (per compatibilita/comodità)

    public class Result<T>
    {
        public Result([CallerMemberName]string Reference = "")
        {
            //if (IsErrorCode(Reference))
            //    this._Reference = Reference;
            //else
            //    this._Reference = Reference.EndsWith("()") ? Reference : string.Concat(Reference, "()");


            if (IsErrorCode(Reference))
                this._Reference = Reference;
            
            //else if (!string.IsNullOrEmpty(Reference))
            //    this._Reference = Reference.EndsWith("()") ? Reference : string.Concat(Reference, "()");

            else
            {
                var methodBase = new StackTrace().GetFrame(1).GetMethod();
                var classBase = methodBase.ReflectedType;
                var nameSpace = (classBase.DeclaringType != null) ? classBase.DeclaringType.FullName : classBase.Namespace;       //OTA:  classBase.Namespace non tiene conto delle sottoclassi.

                this._Reference = $"{nameSpace}.{classBase.Name}.{methodBase.Name}()";
            }


            this.Step = 0;

            this.IsOk = false;
            this.ReturnObject = default(T);
            this.Exception = null;

            this._AdditionalInfo = null;
        }           //COSTRUTTORE

        #region privati

        private const string NO_EXCEPTION_MESSAGE = "[No Exception]";
        private const string ERRORCODE_PREFIX = "EC";
        private string _Reference;
        private string _AdditionalInfo;

        private string FormatSingleOrMultilineMessage(string Message)
        {
            if (string.IsNullOrWhiteSpace(Message))
                return string.Empty;

            if (!Message.Contains("\n"))
                return Message.Trim();

            else
                return
$@"
[
{Indent(Message, 3)}
]";

        }

        private string Indent(string Text, int NumberOfSpaces)
        {
            string spaces = new string(' ', NumberOfSpaces);

            if (string.IsNullOrEmpty(Text))
                return spaces;

            return spaces + Text.Replace("\r\n", "\n").Replace("\n", "\n" + spaces);
        }

        private bool IsErrorCode(string Reference)
        {
            if (string.IsNullOrWhiteSpace(Reference))
                return false;

            if (!Reference.StartsWith(ERRORCODE_PREFIX))
                return false;

            long codeNr;

            if (Reference.Length > ERRORCODE_PREFIX.Length && long.TryParse(Reference.Substring(ERRORCODE_PREFIX.Length), out codeNr) && codeNr >= 0)
                return true;
            else
                return false;
        }

        #endregion

        public bool IsOk { get; set; }
        public int Step { get; set; }
        public T ReturnObject { get; set; }
        public Exception Exception { get; set; }
        public string ErrorMessage
        {
            get
            {
                string exMsg = GetExceptionMessages(Exception);

                string fullMsg = (exMsg == NO_EXCEPTION_MESSAGE) ? NO_EXCEPTION_MESSAGE : //string.Format("REFERENCE: {0}\nSTEP: {1}\nMESSAGE:\n[\n{2}\n]\nADDITIONAL INFO: {3}", _Reference, Step, Indent(exMsg,4), _AdditionalInfo);

$@"REFERENCE: {_Reference}.{Step}
MESSAGE: {FormatSingleOrMultilineMessage(exMsg)}
ADDITIONAL INFO: {_AdditionalInfo}";

                return fullMsg;
            }
        }



        public void ThrowException(string Message = null)
        {
            IsOk = false;
            ReturnObject = default(T);

            if (Message != null)
                Exception = new Exception(Message);                                 //per sicurezza, viene già salvata l'eccezione
            else
                Exception = new Exception(ErrorMessage);                            //propaga eccezione interna (con tutti i sotto-messaggi)

            throw Exception;
        }                       //Crea eccezione con messaggio.

        public void AddInfo(string Info)
        {
            if (_AdditionalInfo == null)
                _AdditionalInfo = Info;
            else
                _AdditionalInfo = string.Concat(_AdditionalInfo, Environment.NewLine, Info);
        }                                        //Aggiunge info addizionali. (Es, parametri della funzione)
        public string GetInfo()
        {
            return _AdditionalInfo;
        }

        //////////////////////////

        public static string GetExceptionMessages(Exception ex)
        {
            if (ex == null)
                return NO_EXCEPTION_MESSAGE;
            else
            {
                Exception _ex = ex;

                #region codice commentato
                //string msg = string.Empty;

                //do
                //{
                //    msg = (!string.IsNullOrWhiteSpace(_ex.Message)) ? (msg + " | " + _ex.Message) : msg;

                //    _ex = _ex.InnerException;
                //}
                //while (_ex != null);
                #endregion

                string msg = _ex.Message;

                _ex = _ex.InnerException;

                while (_ex != null)
                {
                    msg = (!string.IsNullOrWhiteSpace(_ex.Message)) ? (msg + " | " + _ex.Message) : msg;

                    _ex = _ex.InnerException;
                }


                return msg.Trim(' ', '|');
            }
        }                 // STATICO: Recupera/Concatena i messaggi di eccezione (+relative innerexceptions)

    }                                       //Nuova versione con i "generics"

}
