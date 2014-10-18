using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Codeer.Friendly
{
    /// <summary>
    /// Resourcesが使えないのでその対応
    /// </summary>
    static class ResourcesLocal
    {
        internal static IStringResources Instance { get; set; }
        static ResourcesLocal()
        {
            bool isJa = false;
            try
            {
              isJa = (string.Compare(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, "ja", true) == 0);
            }
            catch{}
            Instance = isJa ? (IStringResources)new ResourcesJa() : (IStringResources)new ResourcesEn();
        }
    }

    interface IStringResources
    {
        string ErrorAppCommunication { get; }
        string ErrorAppConnection { get; }
        string ErrorArgumentInvokeFormat { get; }
        string ErrorArgumentInvokeFormatForObjectArray { get; }
        string ErrorAsyncDuplicativeCall { get; }
        string ErrorBinaryInstall { get; }
        string ErrorDefinitionArgument { get; }
        string ErrorDifferentAppFriendVar { get; }
        string ErrorDisposedObject { get; }
        string ErrorDllLoad { get; }
        string ErrorExecuteThreadWindowHandle { get; }
        string ErrorFriendlySystem { get; }
        string ErrorInvalidCompleted { get; }
        string ErrorInvalidStaticCall { get; }
        string ErrorInvalidThreadCall { get; }
        string ErrorManyFoundConstractorFormat { get; }
        string ErrorManyFoundInvokeFormat { get; }
        string ErrorNotFoundConstractorFormat { get; }
        string ErrorNotFoundConstractorFormatForObjectArray { get; }
        string ErrorNotFoundInvokeFormat { get; }
        string ErrorOperationTypeArgInfoFormat { get; }
        string ErrorOperationTypeArgInfoForObjectArrayFormat { get; }
        string ErrorProcessAcess { get; }
        string ErrorProcessOperation { get; }
        string ErrorTargetCpuDifference { get; }
        string ErrorUnpredicatableClrVersion { get; }
        string ExceptionInfoFormat { get; }
        string HasNotEnumerable { get; }
        string NullObjectOperation { get; }
        string ObsoleteClrOrder { get; }
        string OutOfCommunicationNo { get; }
        string OutOfMemory { get; }
        string UnknownTypeInfoFormat { get; }
    }

    class ResourcesEn : IStringResources
    {

        public string ErrorAppCommunication { get { return "Communication with the application failed.\r\nThe target applcation may be unreachable or you may be trying to send\r\ndata that cannot be serialized."; } }
        public string ErrorAppConnection { get { return "Failed to connect to application."; } }
        public string ErrorArgumentInvokeFormat { get { return "[type: {0}][operation: {1} ({2})]\r\nThe specified operation name was found but could not be carried out.\r\nThe arguments could be incorrect.\r\nNote that numerical types and Enums are strictly differentiated.\r\n(For example, even when passing an int as an argument for a long parameter, they are treated as different types and the call fails.)\r\nWhen calling a method with a params argument, please pass the argument as an array value."; } }
        public string ErrorArgumentInvokeFormatForObjectArray { get { return "[type: {0}][operation: {1} ({2})]\r\nAn operation with the specified name was found, but could not be performed.\r\nThe specified arguments may be incorrect.\r\nWhen calling an option with a params argument, please pass the value as an array.\r\nWhen passing object[] as a parameter, this cannot be distinguished from params object[].\r\nPlease pass it as an element of an object[] array in this case.\r\nobject[] arg;        // object[] to pass as a single argument.\r\nobject[] tmpArg = new object[0];\r\ntmpArg [0] = arg;// Please pass tmpArg after doing this"; } }
        public string ErrorAsyncDuplicativeCall { get { return "This has already been executed. An Async object can only be used once. To call an operation more than once, create a new Async object."; } }
        public string ErrorBinaryInstall { get { return "The file in use is in an invalid state and could not be deleted. Please manually delete the following file."; } }
        public string ErrorDefinitionArgument { get { return "Argument number {0} is incorrect. The namespace or class name could be a likely cause. Please double-check the syntax used for creating the argument."; } }
        public string ErrorDifferentAppFriendVar { get { return "Argument number {0} is incorrect. The specified AppVar belongs to a separate AppFriend's variable pool."; } }
        public string ErrorDisposedObject { get { return "It is the object that has been disposed already."; } }
        public string ErrorDllLoad { get { return "Failed to connect to the specified process.\r\nInstallation may have failed."; } }
        public string ErrorExecuteThreadWindowHandle { get { return "Communication with the application failed.\r\nThe indicated window in the target thread does not exist or has already been disposed.\r\nIn applications that display a splash window, the main window may have become a splash window immediately after starting.\r\nPlease specify the handle of the expected window of explicitly."; } }
        public string ErrorFriendlySystem { get { return "Permission to write within the ProgramData folder appears to be denied. Codeer.Friendly creates a ProgramData/Codeer.Friendly and several files within that folder during initialization. Please give your test project write permission to this folder."; } }
        public string ErrorInvalidCompleted { get { return "Invalid completion specification. This method should not generally be called."; } }
        public string ErrorInvalidStaticCall { get { return "Illegal static function call. Operation information requires a type."; } }
        public string ErrorInvalidThreadCall { get { return "There was a call from an unexpected thread."; } }
        public string ErrorManyFoundConstractorFormat { get { return "[new {0}({1})]\r\nMore than one constructor matching the specified arguments was found.\r\nPlease clarify the arguments' types or use OperationTypeInfo."; } }
        public string ErrorManyFoundInvokeFormat { get { return "[type: {0}][operation: {1} ({2})]\r\nMore than one operation matching the specified arguments was found.\r\nPlease clarify arguments' types or use OperationTypeInfo."; } }
        public string ErrorNotFoundConstractorFormat { get { return "[new {0}({1})]\r\nThe constructor was not found.\r\nThe arguments specified may be incorrect.\r\nNote that numerical types and Enums are strictly differentiated.\r\n(For example, even when passing an int as an argument for a long parameter, they are treated as different types and the call fails.)\r\nWhen calling a method with a params argument, please pass the argument as an array value."; } }
        public string ErrorNotFoundConstractorFormatForObjectArray { get { return "[new {0}({1})]\r\nA constructor was not found.\r\nThe arguments specified may be incorrect.\r\nWhen calling an option with a params argument, please pass the value as an array.\r\nWhen passing object[] as a parameter, this cannot be distinguished from params object[].\r\nPlease pass it as an element of an object[] array in this case.\r\nobject[] arg;       // object[] to pass as a singl argument\r\nobject[] tmpArg = new object[0];\r\ntmpArg [0] = arf // please pass tmpArg after doing this"; } }
        public string ErrorNotFoundInvokeFormat { get { return "[type : {0}][operation : {1} ({2})]\r\nThe selected operation was not found."; } }
        public string ErrorOperationTypeArgInfoFormat { get { return "[OperationTypeInfo.Arguments: ({0})][argument : ({1})]\r\nThe specified arguments are incorrect. The number of arguments do not match the number expected by the type.\r\nWhen calling a method with a params parameter, please place the arguments in an array."; } }
        public string ErrorOperationTypeArgInfoForObjectArrayFormat { get { return "[OperationTypeInfo.Arguments: ({0})] [argument : ({1})]\r\nThe specified arguments are incorrect. The number of arguments do not match the number expected by the type.\r\nWhen calling a method with a params parameter, please place the arguments in an array.\r\nWhen passing object[] as a parameter, this cannot be distinguished from params object[].\r\nPlease pass it as an element of an object[] array in this case.\r\nobject[] arg;         // object[] to pass\r\nobject[] tmpArg = new object[0];\r\ntmpArg [0] = arg; // please pass tmpArg after doing this"; } }
        public string ErrorProcessAcess { get { return "Attempt to manipulate the specified process failed. One of the following could be the cause: (1) The specified CLR version is incorrect. (2) Permissions to manipulate the target process are insufficient. (3) The target process terminated during connection. (4) The window for the specified window handle was disposed. \r\nIn applications that display a splash window, the main window may have become a splash window immediately after starting.\r\nPlease specify the handle of the expected window of explicitly."; } }
        public string ErrorProcessOperation { get { return "Permissions to manipulate the target process are insufficient."; } }
        public string ErrorTargetCpuDifference { get { return "Platform targets differ between the test target and test process. Please ensure they are the same."; } }
        public string ErrorUnpredicatableClrVersion { get { return "Failed to detect the CLR version. Multiple CLRs may be loaded in the target application. Please explicitly specify the CLR version in the constructor."; } }
        public string ExceptionInfoFormat { get { return "An exception occurred inside the target application.\r\n[Message]\r\n{0}\r\n[Exception type]\r\n{1}\r\n[Error cause]\r\n{2}\r\n[Stack trace]\r\n{3}\r\n[Help]\r\n{4}"; } }
        public string HasNotEnumerable { get { return "The selected variable does not implement IEnumerable."; } }
        public string NullObjectOperation { get { return "An operation was executed on an AppVar containing a null value."; } }
        public string ObsoleteClrOrder { get { return "The CLR version string was replaced. The specified string is deprecated. Please use the post-replacement version string (official CLR version string) or the version of the constructor that does not take a CLR version."; } }
        public string OutOfCommunicationNo { get { return "The maximum of the number of concurrent transmissions has been exceeded."; } }
        public string OutOfMemory { get { return "The available variable space in the application was exceeded."; } }
        public string UnknownTypeInfoFormat { get { return "[{0}]\r\nThe selected type was not found.\r\nThe specified type's full name is incorrect or the module containing the type has not yet been loaded."; } }
    }

    class ResourcesJa : IStringResources
    {
        public string ErrorAppCommunication { get { return "アプリケーションとの通信に失敗しました。\r\n対象アプリケーションが通信不能な状態になったか、\r\nシリアライズ不可能な型のデータを転送しようとした可能性があります。"; } }
        public string ErrorAppConnection { get { return "アプリケーションとの接続に失敗しました。"; } }
        public string ErrorArgumentInvokeFormat { get { return "[型 : {0}][操作 : {1}({2})]\r\n同名の操作が見つかりましたが、操作を実行できませんでした。\r\n引数指定が間違っている可能性があります。\r\n数値型、Enumも厳密に判定されるのでご注意お願いします。\r\n(例として、long型の引数にint型を渡しても別の型と判断され、解決に失敗します。)\r\nまた、params付きの配列の場合は、その型の配列に格納して渡してください。"; } }
        public string ErrorArgumentInvokeFormatForObjectArray { get { return "[型 : {0}][操作 : {1}({2})]\r\n同名の操作が見つかりましたが、操作を実行できませんでした。\r\n引数指定が間違っている可能性があります。\r\nparams付きの配列の場合は、その型の配列に格納して渡してください。\r\nまた、object[] の場合 params object[]と区別がつかず、分解されて引数に渡されます。\r\nそのため、object[]の要素にobject[]を入れて引き渡してください。\r\nobject[] arg;//引き渡したいobject[]\r\nobject[] tmpArg = new object[0];\r\ntmpArg[0] = arg;//これを引き渡してください。"; } }
        public string ErrorAsyncDuplicativeCall { get { return "すでに実行されています。Asyncクラスの実行は一度だけです。複数回呼び出す場合は、再度Asyncクラスを生成してください。"; } }
        public string ErrorBinaryInstall { get { return "使用するファイルが不正な状態で、かつ削除に失敗しました。\r\n以下のファイルを手動で削除してください。"; } }
        public string ErrorDefinitionArgument { get { return "第{0}引数が不正です。\r\n「ネームスペース」もしくは「クラス名称」と推測されます。引数を生成した構文を確認してください。"; } }
        public string ErrorDifferentAppFriendVar { get { return "第{0}引数が不正です。\r\n引数に使用されたAppVarの中に、異なるAppFriendの管理する変数プールに属するAppVarがあります。"; } }
        public string ErrorDisposedObject { get { return "既に破棄されたオブジェクトです。"; } }
        public string ErrorDllLoad { get { return "指定のプロセスとの接続に失敗しました。\r\nインストールに失敗している可能性があります。"; } }
        public string ErrorExecuteThreadWindowHandle { get { return "アプリケーションとの通信に失敗しました。\r\n指定の実行対象スレッドに含まれるウィンドウは存在しません。\r\nもしくは既に破棄されました。\r\nスプラッシュウィンドウを表示するアプリケーションの場合は、起動直後にメインウィンドウがスプラッシュウィンドウになっている場合があります。\r\n明示的に期待のウィンドウのハンドルを指定してください。"; } }
        public string ErrorFriendlySystem { get { return "ProgramDataフォルダ以下への書き込み権限がないことが考えられます。\r\nCodeer.Friendlyは初期実行時にProgramData/Codeer.Friendlyフォルダとそれ以下にいくつかのファイルを作成します。テストプロジェクトにこのフォルダへの書き込み権限を与えてください。"; } }
        public string ErrorInvalidCompleted { get { return "不正な終了指定です。通常このメソッドは使用しません。"; } }
        public string ErrorInvalidStaticCall { get { return "不正なstatic呼び出しです。操作情報には型が必要です。"; } }
        public string ErrorInvalidThreadCall { get { return "予期せぬスレッドからの呼び出しがありました。"; } }
        public string ErrorManyFoundConstractorFormat { get { return "[new {0}({1})]\r\n指定の引数で実行できる可能性のあるコンストラクタが複数発見されました。\r\n引数に引き渡す型を明確にするか、もしくはOperationTypeInfoを使用してください。"; } }
        public string ErrorManyFoundInvokeFormat { get { return "[型 : {0}][操作 : {1}({2})]\r\n指定の引数で実行できる可能性のある操作が複数発見されました。\r\n引数に引き渡す型を明確にするか、もしくはOperationTypeInfoを使用してください。"; } }
        public string ErrorNotFoundConstractorFormat { get { return "[new {0}({1})]\r\nコンストラクタが見つかりませんでした。\r\n引数指定が間違っている可能性があります。\r\n数値型、Enumも厳密に判定されるのでご注意お願いします。\r\n(例として、long型の引数にint型を渡しても別の型と判断され、解決に失敗します。)\r\nまた、params付きの配列の場合は、その型の配列に格納して渡してください。"; } }
        public string ErrorNotFoundConstractorFormatForObjectArray { get { return "[new {0}({1})]\r\nコンストラクタが見つかりませんでした。\r\n引数指定が間違っている可能性があります。\r\nparams付きの配列の場合は、その型の配列に格納して渡してください。\r\nまた、object[] の場合 params object[]と区別がつかず、分解されて引数に渡されます。\r\nそのため、object[]の要素にobject[]を入れて引き渡してください。\r\nobject[] arg;//引き渡したいobject[]\r\nobject[] tmpArg = new object[0];\r\ntmpArg[0] = arg;//これを引き渡してください。"; } }
        public string ErrorNotFoundInvokeFormat { get { return "[型 : {0}][操作 : {1}({2})]\r\n指定の操作が見つかりませんでした。"; } }
        public string ErrorOperationTypeArgInfoFormat { get { return "[OperationTypeInfo.Arguments : ({0})][引数 : ({1})]\r\n操作型情報の引数指定が不正です。実際に引き渡している引数の数と型指定の数が一致しません。\r\nparams付きの配列の場合は、その型の配列に格納して渡してください。"; } }
        public string ErrorOperationTypeArgInfoForObjectArrayFormat { get { return "[OperationTypeInfo.Arguments : ({0})][引数 : ({1})]\r\n操作型情報の引数指定が不正です。実際に引き渡している引数の数と型指定の数が一致しません。\r\nparams付きの配列の場合は、その型の配列に格納して渡してください。\r\nまた、object[] の場合 params object[]と区別がつかず、分解されて引数に渡されます。\r\nそのため、object[]の要素にobject[]を入れて引き渡してください。\r\nobject[] arg;//引き渡したいobject[]\r\nobject[] tmpArg = new object[0];\r\ntmpArg[0] = arg;//これを引き渡してください。"; } }
        public string ErrorProcessAcess { get { return "対象プロセスの操作に失敗しました。\r\n以下の可能性が考えられます。\r\n・CLRのバージョン指定が間違っている。\r\n・対象プロセスを操作する権限が足りていない。\r\n・接続中に対象プロセスが終了した。\r\n・指定のウィンドウハンドルのウィンドウが破棄された。\r\nスプラッシュウィンドウを表示するアプリケーションの場合は、起動直後にメインウィンドウがスプラッシュウィンドウになっている場合があります。\r\n明示的に期待のウィンドウのハンドルを指定してください。"; } }
        public string ErrorProcessOperation { get { return "対象プロセスを操作する権限が足りていません。"; } }
        public string ErrorTargetCpuDifference { get { return "プラットフォームターゲットがテスト対象とテストプロセスで異なります。合わせてください。"; } }
        public string ErrorUnpredicatableClrVersion { get { return "CLRのバージョンが予測できません。\r\n対象アプリ内に複数のCLRがロードされている可能性があります。\r\n対象のCLRのバージョンを明示するコンストラクタを使用してください。"; } }
        public string ExceptionInfoFormat { get { return "対象アプリケーション内部で例外が発生しました。\r\n[メッセージ]\r\n{0}\r\n[例外タイプ]\r\n{1}\r\n[エラーの原因]\r\n{2}\r\n[スタックトレース]\r\n{3}\r\n[ヘルプ]\r\n{4}"; } }
        public string HasNotEnumerable { get { return "指定の変数はIEnumerableを実装していません。"; } }
        public string NullObjectOperation { get { return "AppVarの中身がnullのオブジェクトに対して操作を呼び出しました。"; } }
        public string ObsoleteClrOrder { get { return "CLRバージョン文字列を置き換えました。\r\n{0} -> {1}\r\n指定の文字列は現在非推奨です。\r\n置き換え後の文字列(CLRの正式バージョン文字列)を使用するか、\r\nCLRのバージョンを指定しないコンストラクタを使ってください。"; } }
        public string OutOfCommunicationNo { get { return "同時通信数の上限に達しました。"; } }
        public string OutOfMemory { get { return "アプリケーション内部で使用できる変数領域を使い切りました。"; } }
        public string UnknownTypeInfoFormat { get { return "[{0}]\r\n指定の型が見つかりません。\r\n型フルネームが間違っているか、指定の型を含むモジュールが、まだロードされていない可能性があります。"; } }
    }
}
