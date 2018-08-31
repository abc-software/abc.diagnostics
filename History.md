FUTURE: PerformanceCounters
FUTURE:	Log4Net
FUTURE:	NLog
FUTURE:	CommonLogging
...

V1.2.1
* Добавлися RoutedLogWriter, что позволить использовать одновременно несколько ILogWriters

V1.2.0
* Перешли на лицензию LGPL V3.0
* Поддержка netstandard
* Добавлись интерфейсы ILogUtility, IExceptionUtility. Все методы как рассширение интерфейсов.

V1.1.0.1/1.0.0.9 
* Поддержка своих атрибутов в конфигурации. ILogWriterAttributes.cs, DiagnosticSettings.cs

V1.1.0.0/1.0.0.8 
* Переработан код для поддержки net45
* Заменен намеспасе на Abc.Diagnostics
* Поддержка EntrLib:
		V60	Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
* Исправлены предупреждения StyleCop

V1.0.0.7
* Поддержка EntrLib:
  * V50U1	Version=5.0.505.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35

V1.0.0.6
* LogActivity, TraceUtility, ExceptionUtility тепреь пишут через LogUtility.Write вместо UseDiagnosticTrace(LogUtility).
* новый статический клас DiagnosticTools
* LogUtility появился новое свойство LogSourceName
* Activity пишется в категорию по умолчанию
* DefaultLogWriter сообщения без категории пишутся в категорию по умолчанию, иначе в категорию Trace.

V1.0.0.5
* DefaultLogWriter поддреживает IsTraceEnabled
* Логирование через LogUtility с Exception должно иметь важность Error
* Поддержка EntrLib:
  * V31	Version=3.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
  * V40	Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
  * V41	Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
  * V50	Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35

V1.0.0.4
* Переведен на VS2010
* Изменен клас TraceUtility, вместо ILogWriter в конструкторе используется UseDiagnosticTrace(LogUtility)

V1.0.0.3
* Реализованы сообщения типа Transfer в DefaultLogWriter.cs, LogActivity.cs
* Добавлены параметры TraceIdentifier, AppDomain в DefaultLogWriter.cs (для удобного просмотра Service Trace Viewer)
* Подправлена в класе LogUtility.cs функция StackTrace для Exception
* ILogWriter добаNEW:	влено свойство и поддержка для IsTracingEnable
* EntrLib40LogWriter теперь пишет в XML формате.
* Разделены при записи в лог Exception и Dictionary<> изменеия в методе ILogWriter.Write.

V1.0.0.2
* LogUtility ArgumentNullException если писать c Exception = null
* Реализован интерфейс IDisposable в EntrLib40LogWriter.cs
* добавлен класс для установки DiagnosticInstaller.cs
* UnmanagedSecurityContextInformationProvider удален, используйте от EnterpriseLibrary
* DebugInformationProvider и ManagedSecurityContextInformationProvider отсоединены от EnterpriseLibrary
* IDebugUtility.cs добавлен интерфейс для DebugInformationProvider

CNG:	Переписан класс DiagnosticSettings с использованием ILogWriter.
* Добавлен класс EntrLib40LogWriter работает c EnterpriseLibrary 4.0
* Добавлен класс DefaultLogWriter работает по умолчанию с XmlWriterTraceListener
* Переписан класс TraceUtility с использованием ILogWriter.
* Добавлен интерфейс ILogWriter.
* Добавлен DiagnosticSettings класс. Теперь можно использовать разные варианты создания логера.

V1.0.0.1
* Добавлен LogActivity класс. Небольшие изменения в ExceptionUtility и LogUtility.
* При записи в EventLog выдавал исключение.
* Добавлен метод WriteCore класу LogUtility, теперь можно наследовать и переобпределять.
* Добавлен ExtraIinformatio
