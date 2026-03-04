<template>
  <div class="min-h-screen flex items-center justify-center bg-slate-50 relative overflow-hidden px-4">
    <div class="absolute top-0 left-0 w-[500px] h-[500px] bg-blue-600/5 rounded-full blur-3xl -translate-x-1/2 -translate-y-1/2"></div>
    
    <div class="max-w-md w-full relative z-10">
      <div class="bg-white/90 backdrop-blur-xl border border-slate-200/60 p-8 rounded-[2rem] shadow-sm relative overflow-hidden group">
        <div class="relative z-10">
          <div class="flex flex-col items-center mb-10">
            <div class="w-16 h-16 bg-emerald-600 rounded-2xl flex items-center justify-center mb-4 shadow-sm shadow-emerald-600/20">
              <RefreshCwIcon class="text-white w-8 h-8" />
            </div>
            <h1 class="text-2xl font-extrabold text-slate-900 mb-2 tracking-tight">Nueva Contraseña</h1>
            <p class="text-slate-500 text-sm font-medium text-center">Ingresá tu código y la nueva contraseña</p>
          </div>

          <div v-if="success" class="text-center space-y-6 animate-in fade-in duration-500">
            <div class="bg-emerald-50 text-emerald-700 p-6 rounded-2xl border border-emerald-100 italic font-medium text-sm">
              {{ message }}
            </div>
            <router-link to="/" class="block text-blue-600 font-bold uppercase tracking-widest text-xs hover:text-blue-500 transition-colors"> Volver al Login </router-link>
          </div>

          <form v-else @submit.prevent="handleSubmit" class="space-y-6">
            <div>
              <label class="block text-slate-400 text-xs font-bold mb-2 uppercase tracking-wide">Correo Electrónico</label>
              <input 
                v-model="data.email" 
                type="email" 
                readonly
                class="w-full bg-slate-100 border border-slate-200/80 text-slate-400 px-4 py-2.5 rounded-xl cursor-not-allowed font-medium"
              />
            </div>

            <div class="relative group">
              <label class="block text-slate-400 text-xs font-bold mb-2 uppercase tracking-wide">Código de Recuperación</label>
              <input 
                v-model="data.token" 
                type="text" 
                required
                maxlength="8"
                class="w-full bg-slate-50/50 border border-slate-200/80 text-slate-900 px-4 py-3.5 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all outline-none font-mono text-center tracking-widest text-lg font-black"
                placeholder="TOKEN123"
              />
            </div>

            <div class="relative group">
              <label class="block text-slate-400 text-xs font-bold mb-2 uppercase tracking-wide">Nueva Contraseña</label>
              <input 
                v-model="data.newPassword" 
                type="password" 
                required
                class="w-full bg-slate-50/50 border border-slate-200/80 text-slate-900 px-4 py-3.5 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all outline-none font-medium"
                placeholder="••••••••"
              />
            </div>

            <button 
              type="submit" 
              :disabled="authStore.loading"
              class="w-full bg-emerald-600 hover:bg-emerald-500 text-white font-medium py-3.5 rounded-xl transition-all active:scale-[0.98] disabled:opacity-50 flex items-center justify-center"
            >
              <span v-if="!authStore.loading">Cambiar Contraseña</span>
              <Loader2Icon v-else class="w-5 h-5 animate-spin" />
            </button>
            <ErrorMessage :message="authStore.error" />
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '../store/auth';
import { RefreshCwIcon, Loader2Icon } from 'lucide-vue-next';
import ErrorMessage from '../components/ErrorMessage.vue';

const authStore = useAuthStore();
const route = useRoute();
const success = ref(false);
const message = ref('');

const data = ref({
  email: '',
  token: '',
  newPassword: ''
});

onMounted(() => {
  data.value.email = route.query.email || '';
  data.value.token = route.query.token || '';
});

const handleSubmit = async () => {
  const result = await authStore.resetPassword(data.value);
  if (result.success) {
    success.value = true;
    message.value = result.message;
  }
};
</script>
