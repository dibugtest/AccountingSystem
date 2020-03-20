
    //function StringToUnicode(data) {
    //    var charCodeUni = "";
    //    for (var i = 0; i < data.length; i++) {
    //        if (data.charCodeAt(i) >= 48 && data.charCodeAt(i) <= 57) {
    //            var charCode = data.charCodeAt(i);
    //            switch (charCode) {
    //                case 48:
    //                    charCodeUni += "०";
    //                    break;
    //                case 49:
    //                    charCodeUni += "१";
    //                    break;
    //                case 50:
    //                    charCodeUni += "२";
    //                    break;
    //                case 51:
    //                    charCodeUni += "३";
    //                    break;
    //                case 52:
    //                    charCodeUni += "४";
    //                    break;
    //                case 53:
    //                    charCodeUni += "५";
    //                    break;
    //                case 54:
    //                    charCodeUni += "६";
    //                    break;
    //                case 55:
    //                    charCodeUni += "७";
    //                    break;
    //                case 56:
    //                    charCodeUni += "८";
    //                    break;
    //                case 57:
    //                    charCodeUni += "९";
    //            }
    //        }
    //        else {
    //            charCodeUni = charCodeUni + data.charAt(i);
    //        }
    //    }
    //    return charCodeUni;
    //    // return String.fromCharCode(charCodeUni.replace(/^"(.*)"$/, '$1'));//regExp to remove the double quotes
    //}

    //function unicodeToString(data) {
    //    var charCodeAsci = "";
    //    for (var i = 0; i < data.length; i++) {
    //        if (data.charCodeAt(i) >= 2406 && data.charCodeAt(i) <= 2415) {
    //            var charCode = data.charCodeAt(i);
    //            switch (charCode) {
    //                case 2406:
    //                    charCodeAsci += "0";
    //                    break;
    //                case 2407:
    //                    charCodeAsci += "1";
    //                    break;
    //                case 2408:
    //                    charCodeAsci += "2";
    //                    break;
    //                case 2409:
    //                    charCodeAsci += "3";
    //                    break;
    //                case 2410:
    //                    charCodeAsci += "4";
    //                    break;
    //                case 2411:
    //                    charCodeAsci += "5";
    //                    break;
    //                case 2412:
    //                    charCodeAsci += "6";
    //                    break;
    //                case 2413:
    //                    charCodeAsci += "7";
    //                    break;
    //                case 2414:
    //                    charCodeAsci += "8";
    //                    break;
    //                case 2415:
    //                    charCodeAsci += "9";
    //            }
    //        }
    //        else {
    //            charCodeAsci = charCodeAsci + data.charAt(i);
    //        }
    //    }
    //    return charCodeAsci;
    //    //return String.fromCharCode(charCodeAsci.replace(/^"(.*)"$/, '$1'));//regExp to remove the double Quotes
    //}
        function AddUnicode(element) {
            google.load("elements", "1", {
                packages: "transliteration"
            });

            var options = {
                sourceLanguage:
                    google.elements.transliteration.LanguageCode.ENGLISH,
                destinationLanguage:
                    [google.elements.transliteration.LanguageCode.NEPALI],
                shortcutKey: 'ctrl+g',
                transliterationEnabled: true
            };

            // Create an instance on TransliterationControl with the required
            // options.
            var control =
                new google.elements.transliteration.TransliterationControl(options);

            control.makeTransliteratable(element);
        }
      