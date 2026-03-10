<template>
  <div class="min-h-screen flex items-center justify-center px-4 transition-colors"
    :class="settingsStore.isDark ? 'bg-slate-950' : 'bg-slate-100'">

    <div class="w-full max-w-sm space-y-6">

      <!-- Header con ícono centrado (igual que login pero sin nombre de app) -->
      <div class="flex flex-col items-center text-center gap-3">
        <div class="w-11 h-11 bg-blue-600 rounded-2xl flex items-center justify-center shadow-lg shadow-blue-600/25">
          <KeyIcon class="w-5 h-5 text-white" />
        </div>
      </div>

      <!-- Card -->
      <div class="rounded-2xl border p-7 transition-colors"
        :class="settingsStore.isDark
          ? 'bg-slate-900 border-slate-800'
          : 'bg-white border-slate-200 shadow-sm'">

        <!-- Título -->
        <div class="mb-6 text-center">
          <h2 class="text-xl font-black"
            :class="settingsStore.isDark ? 'text-white' : 'text-slate-900'">
            Recuperar Acceso
          </h2>
          <p class="text-xs mt-1"
            :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
            Ingresá tu correo y te enviaremos las instrucciones
          </p>
        </div>

        <!-- Estado: enviado -->
        <div v-if="submitted" class="space-y-4">
          <div class="rounded-xl px-4 py-3 text-sm font-medium border"
            :class="settingsStore.isDark
              ? 'bg-emerald-500/10 border-emerald-500/20 text-emerald-400'
              : 'bg-emerald-50 border-emerald-100 text-emerald-700'">
            {{ message }}
          </div>
          <router-link to="/"
            class="block text-center text-[10px] font-black uppercase tracking-widest text-blue-500 hover:text-blue-400 transition-colors mt-2">
            Volver al Login
          </router-link>
        </div>

        <!-- Formulario -->
        <form v-else @submit.prevent="handleSubmit" class="space-y-4">
          <div class="space-y-1.5">
            <label class="text-[10px] font-black uppercase tracking-widest"
              :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
              Correo Electrónico
            </label>
            <input
              v-model="email"
              type="email"
              required
              placeholder="tu@email.com"
              class="w-full border px-4 py-2.5 rounded-xl text-sm outline-none transition-all font-medium"
              :class="settingsStore.isDark
                ? 'bg-slate-800 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
                : 'bg-slate-50 border-slate-200 text-slate-900 placeholder-slate-400 focus:border-blue-500'" />
          </div>

          <button
            type="submit"
            :disabled="authStore.loading"
            class="w-full bg-blue-600 hover:bg-blue-700 active:scale-[0.98] text-white font-black py-2.5 rounded-xl text-xs uppercase tracking-widest transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2 shadow-sm shadow-blue-600/20 mt-2">
            <Loader2Icon v-if="authStore.loading" class="w-4 h-4 animate-spin" />
            <span v-else>Enviar Instrucciones</span>
          </button>

          <router-link to="/"
            class="block text-center text-[10px] font-black uppercase tracking-widest transition-colors mt-1"
            :class="settingsStore.isDark ? 'text-slate-600 hover:text-slate-400' : 'text-slate-400 hover:text-slate-600'">
            Cancelar
          </router-link>
        </form>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useAuthStore } from '../store/auth';
import { useSettingsStore } from '../store/settings';
import { KeyIcon, Loader2Icon } from 'lucide-vue-next';

const authStore     = useAuthStore();
const settingsStore = useSettingsStore();

const email     = ref('');
const submitted = ref(false);
const message   = ref('');

const handleSubmit = async () => {
  const result = await authStore.forgotPassword(email.value);
  submitted.value = true;
  message.value   = result.message;
};
</script>