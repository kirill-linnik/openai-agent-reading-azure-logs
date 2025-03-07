import i18n, { Resource } from "i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import { initReactI18next } from "react-i18next";
import en from "./locales/en/translation.json";
import et from "./locales/et/translation.json";
import { LanguageTypes } from "./models/LanguageTypes";

const resources: Resource = {
  EN: {
    translation: en,
  },
  ET: {
    translation: et,
  },
};

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    detection: {
      order: ["queryString", "cookie"],
      caches: ["cookie"],
    },
    interpolation: {
      escapeValue: false,
    },
    fallbackLng: LanguageTypes.ET,
    supportedLngs: [LanguageTypes.ET, LanguageTypes.EN],
  });

export default i18n;
