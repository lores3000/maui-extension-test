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
		public string Reason { get; set; } = "";
	}

	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);

		var result = await Store(null);
		Console.WriteLine(result.Response);

		CounterBtn.Text = result.Response.ToString() +" "+ result.Reason;
	}

	public async Task<RegistrationResponse> Store(IOrderedEnumerable<KeyValuePair<long, string>> sorted)
	{
		var taskCompletionSource = new TaskCompletionSource<RegistrationResponse>();
		var task = taskCompletionSource.Task;


		CXCallDirectoryManager.SharedInstance.ReloadExtension(
			"ch.bitforge.app.CallDirectoryExtension", 
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

