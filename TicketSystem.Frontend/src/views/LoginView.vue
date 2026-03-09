<template>
  <div class="min-h-screen flex items-center justify-center bg-slate-50 relative overflow-hidden px-4">
    <!-- Premium background atmospheric blobs -->
    <div class="absolute top-0 left-0 w-[500px] h-[500px] bg-blue-600/5 rounded-full blur-3xl -translate-x-1/2 -translate-y-1/2"></div>
    <div class="absolute bottom-0 right-0 w-[600px] h-[600px] bg-indigo-600/5 rounded-full blur-3xl translate-x-1/3 translate-y-1/3"></div>

    <div class="max-w-md w-full relative z-10">
      <div class="bg-white/90 backdrop-blur-xl border border-slate-200/60 p-8 rounded-[2rem] shadow-sm relative overflow-hidden group">
        
        <!-- Inner card decorative blobs -->
        <div class="absolute -top-32 -left-32 w-80 h-80 bg-blue-400/10 rounded-full blur-3xl group-hover:bg-blue-400/20 transition-all duration-700 mix-blend-multiply"></div>
        <div class="absolute -bottom-32 -right-32 w-80 h-80 bg-indigo-400/10 rounded-full blur-3xl group-hover:bg-indigo-400/20 transition-all duration-700 mix-blend-multiply"></div>

        <div class="relative z-10">
          <div class="flex flex-col items-center mb-10">
            <div class="w-16 h-16 bg-blue-600 rounded-2xl flex items-center justify-center mb-4 shadow-sm shadow-blue-600/20">
              <TicketIcon class="text-white w-8 h-8" />
            </div>
            <h1 class="text-3xl font-extrabold text-slate-900 mb-2 tracking-tight">TicketSystem</h1>
            <p class="text-slate-500 text-sm font-medium">Experimentá el soporte premium</p>
          </div>

          <form @submit.prevent="handleLogin" class="space-y-6">
            <div>
              <label class="block text-slate-400 text-xs font-bold mb-2 uppercase tracking-wide">Correo Electrónico</label>
              <div class="relative">
                <input 
                  v-model="email" 
                  type="email" 
                  @input="formErrors.email = ''"
                  class="w-full bg-slate-50/50 border border-slate-200/80 text-slate-900 px-4 py-3.5 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all outline-none placeholder:text-slate-400 font-medium"
                  placeholder="name@company.com"
                />
              </div>
              <p v-if="formErrors.email" class="text-rose-500 text-xs font-medium mt-1.5 flex items-center gap-1">
                <AlertCircleIcon class="w-3.5 h-3.5" /> {{ formErrors.email }}
              </p>
            </div>

            <div>
              <div class="flex justify-between items-center mb-2">
                <label class="block text-slate-400 text-xs font-bold uppercase tracking-wide">Contraseña</label>
                <router-link to="/forgot-password" class="text-blue-500 font-medium text-xs hover:text-blue-600 transition-colors">¿Olvidaste tu contraseña?</router-link>
              </div>
              <input 
                v-model="password" 
                type="password" 
                @input="formErrors.password = ''"
                class="w-full bg-slate-50/50 border border-slate-200/80 text-slate-900 px-4 py-3.5 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all outline-none placeholder:text-slate-400 font-medium"
                placeholder="••••••••"
              />
              <p v-if="formErrors.password" class="text-rose-500 text-xs font-medium mt-1.5 flex items-center gap-1">
                <AlertCircleIcon class="w-3.5 h-3.5" /> {{ formErrors.password }}
              </p>
            </div>

            <div class="flex items-center">
              <input type="checkbox" id="remember" class="w-4 h-4 bg-white border-slate-300 rounded text-blue-600 focus:ring-blue-500" />
              <label for="remember" class="ml-2 text-slate-500 text-sm font-medium cursor-pointer select-none">Recordarme por 30 días</label>
            </div>

            <button 
              type="submit" 
              :disabled="authStore.loading"
              class="w-full bg-blue-600 hover:bg-blue-500 text-white font-medium py-3.5 rounded-xl transition-all active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center"
            >
              <span v-if="!authStore.loading">Iniciar Sesión</span>
              <Loader2Icon v-else class="w-5 h-5 animate-spin" />
            </button>
            <ErrorMessage :message="authStore.error" />
          </form>

          <div class="mt-8 pt-8 border-t border-slate-200 text-center">
            <p class="text-slate-500 font-medium text-sm">
 ¿Nuevo en la plataforma? 
 <router-link to="/register" class="text-blue-400 font-semibold hover:text-blue-300 transition-colors ml-1">Crear una cuenta</router-link>
 </p>
 </div>
 </div>
 </div>
 </div>
 </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '../store/auth';
import { useNotificationStore } from '../store/notifications';
import { TicketIcon, Loader2Icon, AlertCircleIcon } from 'lucide-vue-next';
import ErrorMessage from '../components/ErrorMessage.vue';

const authStore = useAuthStore();
const notificationStore = useNotificationStore();
const router = useRouter();
const route = useRoute();

const email = ref('');
const password = ref('');
const formErrors = ref({ email: '', password: '' });

onMounted(() => {
  const params = new URLSearchParams(window.location.search);
  const msg = params.get('mensaje');
  if (msg) authStore.error = msg;

  if (route.query.expired === 'true') {
    notificationStore.error('Tu sesión ha expirado por seguridad. Por favor, iniciá sesión nuevamente.');
    router.replace('/');
  }
});

const handleLogin = async () => {
  formErrors.value = { email: '', password: '' };
  let hasError = false;

  if (!email.value) {
    formErrors.value.email = 'El correo electrónico es requerido';
    hasError = true;
  }
  if (!password.value) {
    formErrors.value.password = 'La contraseña es requerida';
    hasError = true;
  }

  if (hasError) return;

  const success = await authStore.login(email.value, password.value);
 if (success) {
 notificationStore.success(`¡Bienvenido de nuevo, ${authStore.user?.nombre}!`);
 if (authStore.isAdmin || authStore.isOperador) {
 router.push('/admin');
 } else {
 router.push('/dashboard');
 }
 }
};
</script>

<style scoped>
/* Focus reset for inputs to ensure they don't darken */
input:focus {
  background-color: #ffffff;
}
</style>
