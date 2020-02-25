using Livet.Messaging;
using Livet.Messaging.IO;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Windows;

namespace Prism_MessageBox.ViewModels
{
	public class MainWindowViewModel : BindableBase
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>("MessageBox");

		/// <summary>
		/// Disposeが必要な処理をまとめてやる
		/// </summary>
		private CompositeDisposable Disposable { get; } = new CompositeDisposable();

		/// <summary>
		/// ウィンドウを閉じる時
		/// </summary>
		public ReactiveCommand ClosedCommand { get; } = new ReactiveCommand();

		/// <summary>
		/// Confirmation
		/// </summary>
		public ReactiveCommand ConfirmationCommand { get; } = new ReactiveCommand();

		/// <summary>
		/// Information
		/// </summary>
		public ReactiveCommand InformationCommand { get; } = new ReactiveCommand();

		/// <summary>
		/// OpenFile
		/// </summary>
		public ReactiveCommand OpenFileCommand { get; } = new ReactiveCommand();

		/// <summary>
		/// SaveFile
		/// </summary>
		public ReactiveCommand SaveFileCommand { get; } = new ReactiveCommand();

		/// <summary>
		/// Viewに送るメッセージ
		/// </summary>
		public InteractionMessenger Messenger { get; } = new InteractionMessenger();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel()
		{
			ConfirmationCommand.Subscribe(ConfirmationMethod).AddTo(Disposable);
			InformationCommand.Subscribe(_ => ShowInformationDialog("メッセージ", "タイトル", MessageBoxImage.Error)).AddTo(Disposable);
			OpenFileCommand.Subscribe(OpenFileMethod).AddTo(Disposable);
			SaveFileCommand.Subscribe(SaveFileMethod).AddTo(Disposable);
			ClosedCommand.Subscribe(Close).AddTo(Disposable);
		}

		/// <summary>
		/// アプリが閉じられる時
		/// </summary>
		private void Close()
		{
			Disposable.Dispose();
		}

		#region ↓↓↓↓↓ ダイアログを表示させる基本 ↓↓↓↓↓

		/// <summary>
		/// Confirmationメッセージの表示
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="title">タイトル</param>
		/// <param name="button">表示するボタン</param>
		/// <param name="image">表示するアイコン</param>
		public ConfirmationMessage ShowConfirmationDialog(string message, string title, MessageBoxButton button, MessageBoxImage image)
		{
			ConfirmationMessage msg = new ConfirmationMessage()
			{
				MessageKey = "Confirmation",
				// タイトル
				Caption = title,
				// メッセージ
				Text = message,
				// メッセージアイコン
				Image = image,
				// ボタンの種類
				Button = button
			};
			// 下記でもいいが分かりやすいようにコメントを入れてみる
			// ConfirmationMessage msg = new ConfirmationMessage(message, title, image, button, "Confirmation");
			// 結果が必要な場合はGetResponse(非同期はGetResponseAsync)
			return Messenger.GetResponse(msg);
		}

		/// <summary>
		/// 非同期Informationメッセージの表示
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="title">タイトル</param>
		/// <param name="image">表示するイメージ</param>
		public async void ShowInformationDialog(string message, string title, MessageBoxImage image)
		{
			await Messenger.RaiseAsync(new InformationMessage(message, title, image, "Information"));
		}

		/// <summary>
		/// OpenFileダイアログの表示
		/// </summary>
		public string[] ShowOpenFileDialog()
		{
			OpeningFileSelectionMessage msg = new OpeningFileSelectionMessage()
			{
				MessageKey = "OpenFile",
				// ダイアログのタイトル
				Title = "タイトル",
				// 複数選択の設定
				MultiSelect = true,
				// デフォルトで表示するファイル名
				FileName = "*.*",
				// デフォルトで表示するフォルダ
				InitialDirectory = @"C:\",
				// ファイル種類
				Filter = "全てのファイル(*.*)|*.*|テキストファイル(*.txt)|*.txt"
			};
			// 結果が必要な場合はGetResponse(非同期はGetResponseAsync)
			// 選択されたファイルはresult.Responseに文字列の配列で入る(キャンセル時はnull)
			return Messenger.GetResponse(msg).Response;
		}

		/// <summary>
		/// SaveFileダイアログの表示
		/// </summary>
		/// <returns></returns>
		public string[] SaveFileDialog()
		{
			SavingFileSelectionMessage msg = new SavingFileSelectionMessage()
			{
				MessageKey = "SaveFile",
				// タイトル
				Title = "タイトル",
				// デフォルトで表示するファイル名
				FileName = "*.*",
				// デフォルトで表示するフォルダ
				InitialDirectory = @"C:\",
				// ファイル種類
				Filter = "全てのファイル(*.*)|*.*|テキストファイル(*.txt)|*.txt"
			};
			// 結果が必要な場合はGetResponse(非同期はGetResponseAsync)
			// 選択されたファイルはresult.Responseに文字列の配列で入る(キャンセル時はnull)
			return Messenger.GetResponse(msg).Response;
		}

		#endregion ↑↑↑↑↑ ダイアログを表示させる基本 ↑↑↑↑↑

		/// <summary>
		/// Confirmationメッセージを表示させ、どのボタンを押したかをキャッチする
		/// </summary>
		private void ConfirmationMethod()
		{
			// Yes Noボタンを表示させた場合
			// Yes:result.Response=true No:result.Response=false
			ConfirmationMessage result = ShowConfirmationDialog("メッセージ", "タイトル", MessageBoxButton.YesNo, MessageBoxImage.Question);

			// OK キャンセルを表示させた場合
			// OK:result.Response=true Cancel:result.Response=null
			result = ShowConfirmationDialog("メッセージ", "タイトル", MessageBoxButton.OKCancel, MessageBoxImage.Question);
		}

		/// <summary>
		/// OpenFileメッセージを表示させ、選択されたファイルの情報を取得する
		/// </summary>
		private void OpenFileMethod()
		{
			string[] fileName = ShowOpenFileDialog();
		}

		/// <summary>
		/// SaveFileメッセージを表示させ、選択されたファイル名の情報を取得する
		/// </summary>
		private void SaveFileMethod()
		{
			string[] fileName = SaveFileDialog();
		}

	}
}
