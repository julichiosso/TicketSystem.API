import { defineStore } from 'pinia';

// ── Traducciones ────────────────────────────────────────────────────────────
const translations = {
  es: {
    // Nav / General
    'nav.myTickets':      'Mis Tickets',
    'nav.support':        'Soporte y Seguimiento',
    'nav.settings':       'Configuración',
    'nav.profile':        'Mi Perfil',
    'nav.logout':         'Cerrar Sesión',
    'nav.newTicket':      'Nuevo Ticket',
    'nav.reload':         'Recargar',
    // Dashboard
    'dashboard.greeting':     '¡Hola, {name}!',
    'dashboard.subtitle':     'Gestiona y da seguimiento a tus tickets de soporte',
    'dashboard.avgResponse':  'Tiempo Promedio',
    'dashboard.response':     'Respuesta',
    'dashboard.pending':      'Pendientes',
    'dashboard.inProgress':   'En Proceso',
    'dashboard.resolved':     'Resueltos',
    'dashboard.waiting':      'En espera',
    'dashboard.beingHandled': 'Siendo atendidos',
    'dashboard.completed':    'Completados',
    'dashboard.search':       'Buscar...',
    'dashboard.all':          'Todos',
    'dashboard.tickets':      'tickets',
    'dashboard.emptyTitle':   'Tu buzón está vacío',
    'dashboard.emptyNoResults':'Sin resultados',
    'dashboard.emptyDesc':    'No tenés tickets activos. Creá uno nuevo si necesitás soporte.',
    'dashboard.emptySearch':  'Sin resultados para "{query}".',
    'dashboard.createFirst':  'Crear mi primer ticket',
    'dashboard.page':         'Página',
    'dashboard.of':           'de',
    'dashboard.total':        'total',
    // Nuevo ticket modal
    'ticket.new':          'Nuevo Ticket',
    'ticket.newDesc':      'Describí tu problema con detalle',
    'ticket.subject':      'Asunto',
    'ticket.subjectPH':    'Ej: Error al procesar pago',
    'ticket.description':  'Descripción',
    'ticket.descPH':       "¿Qué está fallando exactamente?",
    'ticket.cancel':       'Cancelar',
    'ticket.send':         'Enviar Solicitud',
    'ticket.sending':      'Enviando...',
    'ticket.low':          'Baja',
    'ticket.medium':       'Media',
    'ticket.urgent':       'Urgente',
    'ticket.errSubject':   'El asunto debe tener al menos 5 caracteres.',
    'ticket.errDesc':      'La descripción debe tener al menos 10 caracteres.',
    'ticket.successSent':  '¡Ticket enviado!',
    'ticket.errSend':      'Error al enviar el ticket.',
    // Chat
    'chat.title':          'Chat de Soporte',
    'chat.empty':          'Sin actividad aún',
    'chat.placeholder':    'Escribe un mensaje...',
    'chat.internal':       'Nota interna...',
    'chat.internalLabel':  'Nota Interna',
    'chat.hint':           'Enter para enviar · Shift+Enter para nueva línea',
    // Settings
    'settings.title':         'Configuración',
    'settings.appearance':    'Modo de Apariencia',
    'settings.light':         'Claro',
    'settings.dark':          'Oscuro',
    'settings.system':        'Sistema',
    'settings.notifications': 'Notificaciones',
    'settings.notifDesc':     'Alertas en tiempo real',
    'settings.language':      'Idioma',
    'settings.security':      'Seguridad',
    'settings.currentPass':   'Contraseña Actual',
    'settings.newPass':       'Contraseña Nueva',
    'settings.confirmPass':   'Confirmar Contraseña',
    'settings.updatePass':    'Actualizar Contraseña',
    'settings.saving':        'Guardando...',
    'settings.passMin':       'Mínimo 6 caracteres',
    'settings.passRepeat':    'Repetí la nueva contraseña',
    'settings.passSuccess':   '¡Contraseña actualizada correctamente!',
    'settings.errFields':     'Completá todos los campos.',
    'settings.errMin':        'La contraseña nueva debe tener al menos 6 caracteres.',
    'settings.errMatch':      'Las contraseñas no coinciden.',
    'settings.errUpdate':     'Error al actualizar la contraseña.',
    // Login
    'login.title':       'Iniciá sesión',
    'login.subtitle':    'Ingresá tu correo y contraseña para continuar',
    'login.email':       'Correo electrónico',
    'login.emailPH':     'nombre@empresa.com',
    'login.password':    'Contraseña',
    'login.forgot':      '¿Olvidaste la tuya?',
    'login.submit':      'Ingresar',
    'login.newUser':     '¿Nuevo en la plataforma?',
    'login.register':    'Crear una cuenta',
    'login.errEmail':    'El correo electrónico es requerido',
    'login.errPassword': 'La contraseña es requerida',
    // Forgot password
    'forgot.title':   'Recuperar Acceso',
    'forgot.subtitle':'Ingresá tu correo y te enviaremos las instrucciones',
    'forgot.email':   'Correo Electrónico',
    'forgot.emailPH': 'tu@email.com',
    'forgot.submit':  'Enviar Instrucciones',
    'forgot.cancel':  'Cancelar',
    'forgot.back':    'Volver al Login',
  },

  en: {
    'nav.myTickets':      'My Tickets',
    'nav.support':        'Support & Tracking',
    'nav.settings':       'Settings',
    'nav.profile':        'My Profile',
    'nav.logout':         'Log Out',
    'nav.newTicket':      'New Ticket',
    'nav.reload':         'Reload',
    'dashboard.greeting':     'Hello, {name}!',
    'dashboard.subtitle':     'Manage and track your support tickets',
    'dashboard.avgResponse':  'Avg Response',
    'dashboard.response':     'Response',
    'dashboard.pending':      'Pending',
    'dashboard.inProgress':   'In Progress',
    'dashboard.resolved':     'Resolved',
    'dashboard.waiting':      'Waiting',
    'dashboard.beingHandled': 'Being handled',
    'dashboard.completed':    'Completed',
    'dashboard.search':       'Search...',
    'dashboard.all':          'All',
    'dashboard.tickets':      'tickets',
    'dashboard.emptyTitle':   'Your inbox is empty',
    'dashboard.emptyNoResults':'No results',
    'dashboard.emptyDesc':    "You have no active tickets. Create one if you need support.",
    'dashboard.emptySearch':  'No results for "{query}".',
    'dashboard.createFirst':  'Create my first ticket',
    'dashboard.page':         'Page',
    'dashboard.of':           'of',
    'dashboard.total':        'total',
    'ticket.new':          'New Ticket',
    'ticket.newDesc':      'Describe your problem in detail',
    'ticket.subject':      'Subject',
    'ticket.subjectPH':    'E.g.: Payment processing error',
    'ticket.description':  'Description',
    'ticket.descPH':       "What's going wrong exactly?",
    'ticket.cancel':       'Cancel',
    'ticket.send':         'Submit Request',
    'ticket.sending':      'Sending...',
    'ticket.low':          'Low',
    'ticket.medium':       'Medium',
    'ticket.urgent':       'Urgent',
    'ticket.errSubject':   'Subject must be at least 5 characters.',
    'ticket.errDesc':      'Description must be at least 10 characters.',
    'ticket.successSent':  'Ticket submitted!',
    'ticket.errSend':      'Error submitting ticket.',
    'chat.title':          'Support Chat',
    'chat.empty':          'No activity yet',
    'chat.placeholder':    'Write a message...',
    'chat.internal':       'Internal note...',
    'chat.internalLabel':  'Internal Note',
    'chat.hint':           'Enter to send · Shift+Enter for new line',
    'settings.title':         'Settings',
    'settings.appearance':    'Appearance Mode',
    'settings.light':         'Light',
    'settings.dark':          'Dark',
    'settings.system':        'System',
    'settings.notifications': 'Notifications',
    'settings.notifDesc':     'Real-time alerts',
    'settings.language':      'Language',
    'settings.security':      'Security',
    'settings.currentPass':   'Current Password',
    'settings.newPass':       'New Password',
    'settings.confirmPass':   'Confirm Password',
    'settings.updatePass':    'Update Password',
    'settings.saving':        'Saving...',
    'settings.passMin':       'At least 6 characters',
    'settings.passRepeat':    'Repeat new password',
    'settings.passSuccess':   'Password updated successfully!',
    'settings.errFields':     'Please fill in all fields.',
    'settings.errMin':        'New password must be at least 6 characters.',
    'settings.errMatch':      "Passwords don't match.",
    'settings.errUpdate':     'Error updating password.',
    'login.title':       'Sign in',
    'login.subtitle':    'Enter your email and password to continue',
    'login.email':       'Email address',
    'login.emailPH':     'name@company.com',
    'login.password':    'Password',
    'login.forgot':      'Forgot yours?',
    'login.submit':      'Sign In',
    'login.newUser':     'New to the platform?',
    'login.register':    'Create an account',
    'login.errEmail':    'Email address is required',
    'login.errPassword': 'Password is required',
    'forgot.title':   'Recover Access',
    'forgot.subtitle':'Enter your email and we will send you instructions',
    'forgot.email':   'Email Address',
    'forgot.emailPH': 'you@email.com',
    'forgot.submit':  'Send Instructions',
    'forgot.cancel':  'Cancel',
    'forgot.back':    'Back to Login',
  },

  fr: {
    'nav.myTickets':      'Mes Tickets',
    'nav.support':        'Support & Suivi',
    'nav.settings':       'Paramètres',
    'nav.profile':        'Mon Profil',
    'nav.logout':         'Déconnexion',
    'nav.newTicket':      'Nouveau Ticket',
    'nav.reload':         'Actualiser',
    'dashboard.greeting':     'Bonjour, {name}!',
    'dashboard.subtitle':     'Gérez et suivez vos tickets de support',
    'dashboard.avgResponse':  'Temps Moyen',
    'dashboard.response':     'Réponse',
    'dashboard.pending':      'En attente',
    'dashboard.inProgress':   'En cours',
    'dashboard.resolved':     'Résolus',
    'dashboard.waiting':      'En attente',
    'dashboard.beingHandled': 'En traitement',
    'dashboard.completed':    'Terminés',
    'dashboard.search':       'Rechercher...',
    'dashboard.all':          'Tous',
    'dashboard.tickets':      'tickets',
    'dashboard.emptyTitle':   'Votre boîte est vide',
    'dashboard.emptyNoResults':'Aucun résultat',
    'dashboard.emptyDesc':    "Vous n'avez pas de tickets actifs. Créez-en un si vous avez besoin d'aide.",
    'dashboard.emptySearch':  'Aucun résultat pour "{query}".',
    'dashboard.createFirst':  'Créer mon premier ticket',
    'dashboard.page':         'Page',
    'dashboard.of':           'sur',
    'dashboard.total':        'total',
    'ticket.new':          'Nouveau Ticket',
    'ticket.newDesc':      'Décrivez votre problème en détail',
    'ticket.subject':      'Sujet',
    'ticket.subjectPH':    'Ex: Erreur de paiement',
    'ticket.description':  'Description',
    'ticket.descPH':       "Qu'est-ce qui ne fonctionne pas exactement?",
    'ticket.cancel':       'Annuler',
    'ticket.send':         'Envoyer',
    'ticket.sending':      'Envoi...',
    'ticket.low':          'Faible',
    'ticket.medium':       'Moyenne',
    'ticket.urgent':       'Urgent',
    'ticket.errSubject':   'Le sujet doit comporter au moins 5 caractères.',
    'ticket.errDesc':      'La description doit comporter au moins 10 caractères.',
    'ticket.successSent':  'Ticket envoyé!',
    'ticket.errSend':      "Erreur lors de l'envoi du ticket.",
    'chat.title':          'Chat de Support',
    'chat.empty':          'Pas encore d\'activité',
    'chat.placeholder':    'Écrire un message...',
    'chat.internal':       'Note interne...',
    'chat.internalLabel':  'Note Interne',
    'chat.hint':           'Entrée pour envoyer · Maj+Entrée pour nouvelle ligne',
    'settings.title':         'Paramètres',
    'settings.appearance':    'Mode d\'apparence',
    'settings.light':         'Clair',
    'settings.dark':          'Sombre',
    'settings.system':        'Système',
    'settings.notifications': 'Notifications',
    'settings.notifDesc':     'Alertes en temps réel',
    'settings.language':      'Langue',
    'settings.security':      'Sécurité',
    'settings.currentPass':   'Mot de passe actuel',
    'settings.newPass':       'Nouveau mot de passe',
    'settings.confirmPass':   'Confirmer le mot de passe',
    'settings.updatePass':    'Mettre à jour',
    'settings.saving':        'Enregistrement...',
    'settings.passMin':       'Au moins 6 caractères',
    'settings.passRepeat':    'Répétez le nouveau mot de passe',
    'settings.passSuccess':   'Mot de passe mis à jour!',
    'settings.errFields':     'Veuillez remplir tous les champs.',
    'settings.errMin':        'Le nouveau mot de passe doit comporter au moins 6 caractères.',
    'settings.errMatch':      'Les mots de passe ne correspondent pas.',
    'settings.errUpdate':     'Erreur lors de la mise à jour du mot de passe.',
    'login.title':       'Se connecter',
    'login.subtitle':    'Entrez votre email et mot de passe pour continuer',
    'login.email':       'Adresse e-mail',
    'login.emailPH':     'nom@entreprise.com',
    'login.password':    'Mot de passe',
    'login.forgot':      'Mot de passe oublié?',
    'login.submit':      'Se connecter',
    'login.newUser':     'Nouveau sur la plateforme?',
    'login.register':    'Créer un compte',
    'login.errEmail':    "L'adresse e-mail est requise",
    'login.errPassword': 'Le mot de passe est requis',
    'forgot.title':   'Récupérer l\'accès',
    'forgot.subtitle':'Entrez votre email et nous vous enverrons les instructions',
    'forgot.email':   'Adresse e-mail',
    'forgot.emailPH': 'vous@email.com',
    'forgot.submit':  'Envoyer les instructions',
    'forgot.cancel':  'Annuler',
    'forgot.back':    'Retour à la connexion',
  }
};

export const useSettingsStore = defineStore('settings', {
  state: () => ({
    themeMode:            localStorage.getItem('themeMode')            || 'light',
    themeColor:           localStorage.getItem('themeColor')           || 'blue',
    uiDensity:            localStorage.getItem('uiDensity')            || 'comfortable',
    language:             localStorage.getItem('language')             || 'es',
    notificationsEnabled: localStorage.getItem('notificationsEnabled') !== 'false',
  }),
  getters: {
    isDark: (state) => state.themeMode === 'dark',
    themeClasses: (state) => {
      const colors = {
        blue:    'from-blue-500 to-blue-700',
        indigo:  'from-indigo-500 to-indigo-700',
        emerald: 'from-emerald-500 to-emerald-700',
        rose:    'from-rose-500 to-rose-700',
        violet:  'from-violet-500 to-violet-700',
      };
      return colors[state.themeColor] || colors.blue;
    },
    accentColor: (state) => {
      const colors = {
        blue: '#3b82f6', indigo: '#6366f1', emerald: '#10b981',
        rose: '#f43f5e', violet: '#8b5cf6',
      };
      return colors[state.themeColor] || colors.blue;
    },
    // Función de traducción — usá: settingsStore.t('key') o settingsStore.t('key', { name: 'Juan' })
    t: (state) => (key, vars = {}) => {
      const lang  = state.language || 'es';
      const dict  = translations[lang] || translations.es;
      let text    = dict[key] ?? translations.es[key] ?? key;
      Object.entries(vars).forEach(([k, v]) => {
        text = text.replace(`{${k}}`, v);
      });
      return text;
    },
  },
  actions: {
    setThemeMode(mode) {
      this.themeMode = mode;
      localStorage.setItem('themeMode', mode);
      this.applyTheme();
    },
    toggleThemeMode() {
      this.setThemeMode(this.themeMode === 'light' ? 'dark' : 'light');
    },
    setThemeColor(color) {
      this.themeColor = color;
      localStorage.setItem('themeColor', color);
      this.applyAccentColor();
    },
    setUIDensity(density) {
      this.uiDensity = density;
      localStorage.setItem('uiDensity', density);
      document.documentElement.setAttribute('data-density', density);
    },
    setLanguage(lang) {
      this.language = lang;
      localStorage.setItem('language', lang);
    },
    toggleNotifications() {
      this.notificationsEnabled = !this.notificationsEnabled;
      localStorage.setItem('notificationsEnabled', this.notificationsEnabled);
    },
    applyAccentColor() {
      const colors = {
        blue: '59 130 246', indigo: '99 102 241', emerald: '16 185 129',
        rose: '244 63 94',  violet: '139 92 246',
      };
      document.documentElement.style.setProperty('--accent-primary', colors[this.themeColor] || colors.blue);
    },
    applyTheme() {
      const isDark = this.themeMode === 'dark';
      document.documentElement.classList.toggle('dark', isDark);
      document.documentElement.setAttribute('data-theme', this.themeMode);
    },
    initSettings() {
      this.applyTheme();
      this.applyAccentColor();
      document.documentElement.setAttribute('data-density', this.uiDensity);
    }
  }
});