using CallKit;

namespace app;

public partial class MainPage : ContentPage
{
	public class RegistrationResponse
	{
		public enum RegistrationResponseEnum
		{
			Ok,
			Error,
			NotActivated
		}

		public RegistrationResponseEnum Response { get; set; }
		public string Reason { get; set; }
	}

	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}


        public async Task<RegistrationResponse> Store(IOrderedEnumerable<KeyValuePair<long, string>> sorted)
        {
			/*
            var arr = sorted.ToArray();
            const int EntriesPerDictionary = 100000; //create separate files for subsets of entries
            const string delimiter = ";";

            for (int i = 0; i < 1000; i++)
            {

                var fileName = GetPhoneFilePath(i);

                // Read all existing files
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                else
                {
                    //Console.WriteLine("Deleted " + i + " files");
                    break;
                }
            }

            for (int i=0; i < arr.Length / EntriesPerDictionary+1; i++){

                var fileName = GetPhoneFilePath(i);
                //var dictionary = new NSMutableDictionary();
                var lines = new List<string>();

                for (int j = 0; j<EntriesPerDictionary && j+i* EntriesPerDictionary < arr.Length; j++)
                {
                    var entry = arr[j + i * EntriesPerDictionary];
                    lines.Add($"{entry.Key}{delimiter}{entry.Value}");
                }
                File.WriteAllLines(fileName, lines.ToArray());


                Console.WriteLine($"AddIdentificationEntry fileName {fileName}");
                //Console.WriteLine("PhonenumberDirectory" + i);

            }*/

            var taskCompletionSource = new TaskCompletionSource<RegistrationResponse>();
            var task = taskCompletionSource.Task;
            //Action<RegistrationResponse> callback = taskCompletionSource.SetResult;


            CXCallDirectoryManager.SharedInstance.ReloadExtension(
                "ch.usz.internal.directory.callkitextension", 
                error => 
                { 
                    if (error == null) 
                    {
                        // Reloaded extension successfully
                        taskCompletionSource.SetResult(new RegistrationResponse { Response = RegistrationResponse.RegistrationResponseEnum.Ok });
                    } else {
                        Console.WriteLine($"Error activating Extension {error.Code}:{error.Description}");

                        switch ((CXErrorCodeCallDirectoryManagerError)(int)error.Code)
                        {
                            case CXErrorCodeCallDirectoryManagerError.ExtensionDisabled:
                                taskCompletionSource.SetResult(new RegistrationResponse { Response = RegistrationResponse.RegistrationResponseEnum.NotActivated });
                                return;
                            case CXErrorCodeCallDirectoryManagerError.Unknown:
                            case CXErrorCodeCallDirectoryManagerError.NoExtensionFound:
                            case CXErrorCodeCallDirectoryManagerError.LoadingInterrupted:
                            case CXErrorCodeCallDirectoryManagerError.EntriesOutOfOrder:
                            case CXErrorCodeCallDirectoryManagerError.DuplicateEntries:
                            case CXErrorCodeCallDirectoryManagerError.MaximumEntriesExceeded:
                            case CXErrorCodeCallDirectoryManagerError.CurrentlyLoading:
                            case CXErrorCodeCallDirectoryManagerError.UnexpectedIncrementalRemoval:
                                Console.WriteLine(error);
                                taskCompletionSource.SetResult(new RegistrationResponse {
                                    Response =
                                    RegistrationResponse.RegistrationResponseEnum.Error,
                                    Reason = "" + error.Code
                                });
                                return;
                        }
                        taskCompletionSource.SetResult(
                            new RegistrationResponse {
                                Response = RegistrationResponse.RegistrationResponseEnum.Error,
                                Reason = ""+error.Code
                            });
                    }
                }
            );

            return await Task.FromResult(task.Result);
        }
}

