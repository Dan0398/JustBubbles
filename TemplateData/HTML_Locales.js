var UnpackingLabel = '';

function UpdateLanguageByLangCode(Code)
{
	if (Code === 'ru')
	{
		MyEnv.LanguageCode = Code;
		UnpackingLabel = 'Распаковка...';
	}
	else if (Code === 'tr') //Турецкий
	{
		MyEnv.LanguageCode = Code;
		UnpackingLabel = 'Ambalajın açılması...';
	}
	
	else if (Code === 'es') //Испанский
	{
		MyEnv.LanguageCode = Code;
		UnpackingLabel = 'Desembalaje...';
	}
	
	else if (Code === 'pt') //Португальский
	{
		MyEnv.LanguageCode = Code;
		UnpackingLabel = 'Desembalar...';
	}
	
	else if (Code === 'fr')//Французский
	{
		MyEnv.LanguageCode = Code;
		UnpackingLabel = 'Déballage...';
	}
	
	else if (Code === 'it')//Итальянский
	{
		MyEnv.LanguageCode = Code;
		UnpackingLabel = 'Disimballaggio...';
	}
	
	else if (Code === 'de') //Немецкий
	{
		MyEnv.LanguageCode = Code;
		UnpackingLabel = 'Auspacken...';
	}
	else //Английский или любой другой
	{
		MyEnv.LanguageCode = "en";
		UnpackingLabel = 'Unpacking...';
	}
	UpdateLoadingBar();
}
