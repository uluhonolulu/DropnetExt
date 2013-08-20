using DropNet;
using DropNet.Exceptions;

namespace ChpokkWeb.Features.Remotes.Dropbox {
	public class DropnetWrapper {
		private readonly DropNetClient _client;
		State _state = State.Uninitialized;
		public DropnetWrapper() {
			_client = new DropNetClient(DropboxAuth.API_KEY, DropboxAuth.APP_SECRET);
		}

		/// <summary>
		/// Use the dropbox client only after you have authorized your application.
		/// </summary>
		public DropNetClient Client {
			get {
				switch (_state) {
					case State.Uninitialized:
						throw new DropboxException("You must be authorized to use Dropbox");
					case State.GotRequestToken:
						//assume that we have visited the Authorization Url
						_client.GetAccessToken();
						_state = State.Ready;
						break;
					case State.Ready:
						break;
				}
				return _client;
			}
		}

		/// <summary>
		/// Call this method in order to get the Dropbox authorization Url
		/// </summary>
		/// <param name="callback">An Url on your site that you want to redirect to after the user authorizes herself on the Dropbox site</param>
		/// <returns>The Url on the Dropbox site that would let the user authorize your app to use Dropbox</returns>
		/// <remarks>Before using the API, you should call this method and open the returned URL in a browser window, so that the user can allow access to her Dropbox files for your application.</remarks>
		public string GetAuthorizeUrl(string callback = null) {
			_state = State.GotRequestToken;
			return _client.GetTokenAndBuildUrl(callback);
		}



		enum State {
			Uninitialized,
			GotRequestToken,
			Ready
		}
	}

	public struct DropboxAuth {
		//public const string API_KEY = "API_KEY";
		//public const string APP_SECRET = "APP_SECRET";
	}
}