<template>
  <div class="min-h-screen flex items-center justify-center bg-slate-50 relative overflow-hidden px-4">
    <div class="absolute top-0 left-0 w-[500px] h-[500px] bg-blue-600/5 rounded-full blur-3xl -translate-x-1/2 -translate-y-1/2"></div>
    
    <div class="max-w-md w-full relative z-10">
      <div class="bg-white/90 backdrop-blur-xl border border-slate-200/60 p-8 rounded-[2rem] shadow-sm relative overflow-hidden group">
        <div class="relative z-10">
          <div class="flex flex-col items-center mb-10">
            <div class="w-16 h-16 bg-blue-600 rounded-2xl flex items-center justify-center mb-4 shadow-sm shadow-blue-600/20">
              <KeyIcon class="text-white w-8 h-8" />
            </div>
            <h1 class="text-2xl font-extrabold text-slate-900 mb-2 tracking-tight">Recuperar Acceso</h1>
            <p class="text-slate-500 text-sm font-medium text-center">Ingresá tu correo y te enviaremos las instrucciones</p>
          </div>

          <div v-if="submitted" class="text-center space-y-6 animate-in fade-in duration-500">
            <div class="bg-emerald-50 text-emerald-700 p-6 rounded-2xl border border-emerald-100 italic font-medium text-sm">
              {{ message }}
            </div>
            <router-link to="/" class="block text-blue-600 font-bold uppercase tracking-widest text-xs hover:text-blue-500 transition-colors"> Volver al Login </router-link>
          </div>

          <form v-else @submit.prevent="handleSubmit" class="space-y-6">
            <div>
              <label class="block text-slate-400 text-xs font-bold mb-2 uppercase tracking-wide">Correo Electrónico</label>
              <input 
                v-model="email" 
                type="email" 
                required
                class="w-full bg-slate-50/50 border border-slate-200/80 text-slate-900 px-4 py-3.5 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all outline-none font-medium"
                placeholder="tu@email.com"
              />
            </div>

            <button 
              type="submit" 
              :disabled="authStore.loading"
              class="w-full bg-blue-600 hover:bg-blue-500 text-white font-medium py-3.5 rounded-xl transition-all active:scale-[0.98] disabled:opacity-50 flex items-center justify-center"
            >
              <span v-if="!authStore.loading">Enviar Instrucciones</span>
              <Loader2Icon v-else class="w-5 h-5 animate-spin" />
            </button>

            <router-link to="/" class="block text-center text-slate-500 font-bold uppercase tracking-widest text-[10px] hover:text-slate-900 transition-colors"> Cancelar </router-link>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useAuthStore } from '../store/auth';
import { KeyIcon, Loader2Icon } from 'lucide-vue-next';

const authStore = useAuthStore();
const email = ref('');
const submitted = ref(false);
const message = ref('');

const handleSubmit = async () => {
  const result = await authStore.forgotPassword(email.value);
  submitted.value = true;
  message.value = result.message;
};
</script>
