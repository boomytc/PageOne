using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Text;

namespace rui
{
    /// <summary>
    /// ��־���������
    /// </summary>
    public class logHelper
    {
        /// <summary>
        /// ��¼�ı�����
        /// ������ߺ����
        /// </summary>
        /// <param name="message">��¼���ı�����</param>
        public static void log(string message)
        {
            logHelper.Instance.LogMsg(message);
        }

        /// <summary>
        /// ��¼�쳣����
        /// ������Զ�����쳣�����ߺ󲻼�¼
        /// </summary>
        /// <param name="ex">��¼���쳣����</param>
        /// <param name="isThrow">�Ƿ���������׳����쳣</param>
        public static void log(Exception ex,bool isThrow=false)
        {
            if(ex != null)
                logHelper.Instance.LogException(ex);
            if (ex.InnerException != null)
                logHelper.Instance.LogException(ex.InnerException);

            if (isThrow == true)
                throw ex;
        }

        /// <summary>
        /// ��¼���󣬼�¼�쳣
        /// ��������ˣ����󲻼�¼���Զ����쳣����¼
        /// </summary>
        /// <param name="message">��¼���ı�����</param>
        /// <param name="ex">��¼���쳣����</param>
        /// <param name="isThrow">�Ƿ���������׳����쳣</param>
        public static void log(string message, Exception ex, bool isThrow = false)
        {
            log(message);
            log(ex, isThrow);
        }

        /// <summary>
        /// ���������Ϣ
        /// ���ߺ�Ҳ���
        /// </summary>
        /// <param name="message">���Ե��ı�����</param>
        public static void debug(string message)
        {
            try
            {
                logHelper.Instance.LogDebug(message);
            }
            catch
            {

            }
        }

        /// <summary>
        /// ��¼�ַ������� - ���ߺ����
        /// </summary>
        /// <param name="message"></param>
        private void LogMsg(string message)
        {
            try
            {
                if (rui.configHelper.isOnline != "true")
                    logger.Info(message);
            }
            catch
            {

            }
        }

        /// <summary>
        /// ��¼�ַ������� - ���ߺ����
        /// </summary>
        /// <param name="message"></param>
        private void LogDebug(string message)
        {
            try
            {
                logger.Info(message);
            }
            catch
            {

            }
        }

        /// <summary>
        /// ��¼�쳣����
        /// ������Զ�����쳣�������ߺ󲻼�¼
        /// </summary>
        /// <param name="ex"></param>
        private void LogException(Exception ex)
        {
            try
            {
                //������Զ�����쳣���򲻽��м�¼
                if(ex.GetType().ToString() != "rui.ruiException")
                {
                    logger.Error(getExInfo(ex));
                }
                if(ex.GetType().ToString() == "rui.ruiException" && rui.configHelper.isShowRuiException == "��")
                {
                    logger.Error(getExInfo(ex));
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// ��ȡ�쳣��Ϣ
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string getExInfo(Exception ex)
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine(ex.Message);
            info.AppendLine("------------begin-------------");
            info.AppendLine(ex.StackTrace);
            info.AppendLine("------------End-------------");
            return info.ToString();
        }

        private Logger logger;
        private logHelper()
        {
            LogDate = DateTime.Now;
            SimpleConfigurator.ConfigureForTargetLogging(GetFileTarget());
            logger = LogManager.GetCurrentClassLogger();
        }
        private DateTime logDate;
        private DateTime LogDate
        {
            get { return logDate; }
            set { logDate = value; }
        }
        //��������
        private static logHelper _Instance;
        private static logHelper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new logHelper();
                }
                else
                {
                    if (_Instance.logDate.Date != DateTime.Now.Date)
                    {
                        _Instance = new logHelper();
                    }
                }
                return _Instance;
            }
        }
        //��ȡһ��Log�ļ�
        private string GetLogFile()
        {
            string fileName = "Log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "log", fileName);
        }
        //����Log�����Ŀ����Ϣ
        private FileTarget GetFileTarget()
        {
            FileTarget ft = new FileTarget();
            ft.Encoding = "UTF-8";
            ft.FileName = GetLogFile();
            ft.Layout = "${longdate} ${level} ${message} ";
            ft.KeepFileOpen = false;
            ft.OpenFileCacheTimeout = 10;
            ft.OpenFileCacheSize = 1;
            return ft;
        }
    }
}
