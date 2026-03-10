import { useSettingsStore } from '../store/settings';

const translations = {
    es: {
        auth: {
            sign_in: 'Iniciar sesión',
            continue_to: 'para continuar a TicketSystem.',
            email_phone: 'Ruta, correo electrónico o teléfono',
            password: 'Contraseña',
            forgot: '¿Olvidó su contraseña?',
            next: 'Siguiente',
            no_account: '¿No tiene una cuenta?',
            create_one: 'Cree una.',
            email_req: 'Se requiere el correo electrónico',
            pass_req: 'Se requiere la contraseña',
            loading: 'Cargando...',
            login_options: 'Opciones de inicio de sesión',
            new_account: 'Crear cuenta',
            join: 'Unite a la plataforma de soporte.',
            fullname: 'Nombre y apellido',
            already_have: '¿Ya tiene una cuenta?',
            back_to_login: 'Iniciar sesión',
        }
    },
    en: {
        auth: {
            sign_in: 'Sign in',
            continue_to: 'to continue to TicketSystem.',
            email_phone: 'Email, phone, or Skype',
            password: 'Password',
            forgot: 'Forgot password?',
            next: 'Next',
            no_account: 'No account?',
            create_one: 'Create one!',
            email_req: 'Email is required',
            pass_req: 'Password is required',
            loading: 'Loading...',
            login_options: 'Sign-in options',
            new_account: 'Create account',
            join: 'Join the support platform.',
            fullname: 'Full Name',
            already_have: 'Already have an account?',
            back_to_login: 'Sign in',
        }
    },
    fr: {
        auth: {
            sign_in: 'Se connecter',
            continue_to: 'pour accéder à TicketSystem.',
            email_phone: 'E-mail, téléphone ou Skype',
            password: 'Mot de passe',
            forgot: 'Mot de passe oublié ?',
            next: 'Suivant',
            no_account: 'Pas de compte ?',
            create_one: 'Créez-en un !',
            email_req: 'L\'e-mail est requis',
            pass_req: 'Le mot de passe est requis',
            loading: 'Chargement...',
            login_options: 'Options de connexion',
            new_account: 'Créer un compte',
            join: 'Rejoindre la plateforme.',
            fullname: 'Nom complet',
            already_have: 'Vous avez déjà un compte ?',
            back_to_login: 'Se connecter',
        }
    }
};

export function useI18n() {
    const settingsStore = useSettingsStore();

    const t = (key) => {
        const keys = key.split('.');
        let val = translations[settingsStore.language || 'es'];
        for (const k of keys) {
            if (!val) return key;
            val = val[k];
        }
        return val;
    };

    return { t };
}
