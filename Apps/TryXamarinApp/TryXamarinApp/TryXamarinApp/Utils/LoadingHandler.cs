using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TryXamarinApp.Views;
using Xamarin.Forms;

namespace TryXamarinApp.Utils
{
	class LoadingHandler
	{

		public static async Task StartLoading(INavigation navigation, string message)
		{
			ContentPage page = new LoadingPage(message);
			await navigation.PushAsync(page);
		}
		public static async Task FinishLoading(INavigation navigation)
		{
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
