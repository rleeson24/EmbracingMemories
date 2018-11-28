
module Profiles {
	export class QrProfileModel {
		Id: Number;
		Links: ILink[];
		FirstName: String;
		MiddleName: String;
		LastName: String;
		Birthday: Date;
		DateOfDeath: Date;
		Obituary: String;
		LifeHistory: String;
		Sex: String;
		Photos: IPhoto[];
		AudioFiles: IAudioFile[];
		Videos: IVideoFile[];
	}

	export interface IBaseProfileScope {
		model: QrProfileModel;
	}
	
	export class CreateProfileModel {
		UserFirstName: String;
		UserLastName: String;
		UserAddressLine1: String;
		UserAddressLine2: String;
		UserCity: String;
		UserState: String;
		UserPostalCode: String;
		UserCountry: String;
		UserPhone: String;
		
		FirstName: String;
		MiddleName: String;
		LastName: String;
		Sex: String;
		Birthday: Date;
		DateOfDeath: Date;
		UserEmail: String;
		UserEmailConfirmation: String;
		PaymentError: String;
		CardToken: String;

		constructor() {
			this.UserFirstName = '';
			this.UserLastName = '';
			this.UserAddressLine1 = '';
			this.UserAddressLine2 = '';
			this.UserCity = '';
			this.UserState = '';
			this.UserPostalCode = '';
			this.UserCountry = 'US';
			this.UserPhone = '';
			this.FirstName = '';
			this.MiddleName = '';
			this.LastName = '';
			this.Sex = '';
			this.Birthday = null;
			this.DateOfDeath = null;
			this.UserEmail = '';
			this.UserEmailConfirmation = '';
			this.PaymentError = '';
			this.CardToken = null;
		}
	}

	export interface ICreateProfileScope {
		ShowPageMessage: Boolean;
		Message: String;
		RequestInProgress: Boolean;
		Errors: Object[];
		model: CreateProfileModel;
		saveProfile: Function;
		showSaveComplete: Boolean;
		showPaymentForm: Boolean;
		success: Boolean;
		submitPaymentData: Function;
		StatesList: Geography.StateDefinition[];
		init: Function;
		$on: Function;
		$apply: Function;
	}

	export interface IAudioFile {
		Id: String;
		Url: String;
	}

	export interface IVideoFile {
		Id: String;
		Url: String;
	}

	export interface IPhoto {
		Id: String;
	}

	export interface ILink {
		label: String;
        url: String;
	}

	export interface IViewProfileScope extends IBaseProfileScope {
		getProfile: Function;
		images: Object[];
		AudioFiles: IAudioFile[];
	}

	export interface IEditProfileScope {
		ShowPageMessage: Boolean;
		Message: String;
		RequestInProgress: Boolean;
		Errors: Object[];
		newPhoto: any;
		model: QrProfileModel;
		addLink: Function;
		deleteLink: Function;
		getProfile: Function;
		saveProfile: Function;
		uploadPhoto: Function;
		deletePhoto: Function;
		deleteAudioFile: Function;
		deleteVideo: Function;
		showSaveComplete: Boolean;
		success: Boolean;
		$watch: Function;
	}

	export interface IProfileListScope {
		createNew: Function;
		ShowPageMessage: Boolean;
		Message: String;
		Busy: Boolean;
		Profiles: QrProfileModel[];
		$on: Function;
	}
}