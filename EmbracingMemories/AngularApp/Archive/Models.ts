module Archive {
	export interface IArchiveScope {
		ShowPageMessage: Boolean;
		Message: String;
		RequestInProgress: Boolean;
		Errors: Object[];
		PaymentModel: PaymentModel;
		ListModel: ListModel;
		showSaveComplete: Boolean;
		success: Boolean;
		submitPaymentData: Function;
		showModal: Boolean;
		logoffDialogOk: Function;
		saveNew: Function;
		setSelectedEntry: Function;
		$on: Function;
		$apply: Function;
		$watch: Function;
	}

	export class PaymentModel {
		CreditCardNumber: String;
		CreditCardExpirationMonth: String;
		CreditCardExpirationYear: String;
		CreditCardPostalCode: String;
		PaymentError: String;
		CardToken: String;
	}

	export class ListModel {
		ArchiveEntries: ArchiveModel[];
		NewEntry: ArchiveModel;
		SelectedEntry: ArchiveModel;
	}

	export class ArchiveModel {
		Id: String;
		FirstName: String;
		MiddleName: String;
		LastName: String;
		Suffix: String;
		VideoUrl: String;
	}
}