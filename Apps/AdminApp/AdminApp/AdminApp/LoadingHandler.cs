using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AdminApp
{
	class LoadingHandler
	{
		private static bool currentlyLoading;
		public static async Task StartLoading(INavigation navigation, string message)
		{
			if (currentlyLoading) return;
			currentlyLoading = true;
			ContentPage page = new LoadingPage(message);
			await navigation.PushAsync(page);
		}
		public static async Task FinishLoading(INavigation navigation)
		{
			if(!currentlyLoading) return;
			currentlyLoading=false;
			await navigation.PopAsync();
		}

		//public static async Task LoadScreen<Tout>(INavigation navigation, string message, Func<Task<Tout>> f)
		//{
		//	await StartLoading(navigation, message);
		//	await f();
		//	await FinishLoading(navigation);
		//}
	

		//public static async Task LoadScreen(INavigation navigation, string message, Func<Task> f)
		//{
		//	await StartLoading(navigation, message);
		//	await f();
		//	await FinishLoading(navigation);
		//}

		/*** Python code ***/
		//
		//def f(n):
		//	msg = 'public static async Task LoadScreen<'
		//	msg += ', '.join([f'Tin{i}' for i in range(n)])
		//	msg+='>'
		//	msg += '(INavigation navigation, string message, Func<'
		//	for i in range(n):
		//		msg+=f'Tin{i},'
		//	msg+= 'Task> f'
		//	for i in range(n):
		//		msg+=f', Tin{i} input{i}'
		//	msg+=')'
		//	msg+="""{
		//	await StartLoading(navigation, message);
		//	await f("""
		//	msg += ', '.join([f'input{i}' for i in range(n)])
		//	msg+=""");
		//	await FinishLoading(navigation);
		//}"""
		//	return msg
		//
		//for i in range(1,16):
		//	print(f(i))
	}
}
