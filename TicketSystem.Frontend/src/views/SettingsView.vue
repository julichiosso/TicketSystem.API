<template>
  <div class="min-h-screen p-10 font-sans transition-colors duration-300"
    :class="settingsStore.isDark ? 'bg-slate-950 text-white' : 'bg-slate-50 text-slate-900'">
    <div class="max-w-4xl mx-auto">

      <!-- Header -->
      <div class="flex items-center gap-4 mb-12">
        <button @click="router.back()"
          class="p-3 rounded-2xl transition-all"
          :class="settingsStore.isDark
            ? 'bg-slate-800 hover:bg-slate-700 text-slate-300'
            : 'bg-white hover:bg-slate-100 text-slate-500 border border-slate-200'">
          <ArrowLeftIcon class="w-5 h-5" />
        </button>
        <h2 class="text-4xl font-black italic tracking-tighter uppercase">Configuración</h2>
      </div>

      <div class="space-y-6">

        <!-- ── MODO APARIENCIA ── -->
        <section class="rounded-2xl p-6 border transition-colors"
          :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200'">
          <div class="flex items-center gap-3 mb-6">
            <SunMoonIcon class="w-5 h-5 text-blue-500" />
            <h3 class="text-sm font-black uppercase tracking-widest">Modo de Apariencia</h3>
          </div>

          <div class="grid grid-cols-2 gap-4">
            <!-- Claro -->
            <button @click="settingsStore.setThemeMode('light')"
              class="relative rounded-xl overflow-hidden border-2 transition-all group"
              :class="settingsStore.themeMode === 'light'
                ? 'border-blue-500'
                : settingsStore.isDark ? 'border-slate-700 hover:border-slate-600' : 'border-slate-200 hover:border-slate-300'">
              <!-- Preview claro -->
              <div class="bg-slate-100 p-3 h-36 flex flex-col gap-2">
                <!-- Fake topbar -->
                <div class="bg-white rounded-lg px-3 py-1.5 flex items-center gap-2 shadow-sm">
                  <div class="w-3 h-3 rounded bg-blue-500"></div>
                  <div class="flex-1 h-1.5 bg-slate-200 rounded-full"></div>
                  <div class="w-4 h-4 rounded-full bg-slate-200"></div>
                </div>
                <!-- Fake cards -->
                <div class="flex gap-2 flex-1">
                  <div class="w-12 bg-white rounded-lg shadow-sm"></div>
                  <div class="flex-1 flex flex-col gap-1.5">
                    <div class="bg-white rounded-lg h-8 shadow-sm"></div>
                    <div class="bg-white rounded-lg flex-1 shadow-sm"></div>
                  </div>
                </div>
              </div>
              <div class="py-2.5 flex items-center justify-center gap-2"
                :class="settingsStore.isDark ? 'bg-slate-800' : 'bg-white'">
                <SunIcon class="w-4 h-4 text-amber-400" />
                <span class="text-xs font-black uppercase tracking-widest">Claro</span>
                <div v-if="settingsStore.themeMode === 'light'"
                  class="w-4 h-4 rounded-full bg-blue-500 flex items-center justify-center ml-1">
                  <CheckIcon class="w-2.5 h-2.5 text-white" />
                </div>
              </div>
            </button>

            <!-- Oscuro -->
            <button @click="settingsStore.setThemeMode('dark')"
              class="relative rounded-xl overflow-hidden border-2 transition-all group"
              :class="settingsStore.themeMode === 'dark'
                ? 'border-blue-500'
                : settingsStore.isDark ? 'border-slate-700 hover:border-slate-600' : 'border-slate-200 hover:border-slate-300'">
              <!-- Preview oscuro -->
              <div class="bg-slate-950 p-3 h-36 flex flex-col gap-2">
                <!-- Fake topbar -->
                <div class="bg-slate-800 rounded-lg px-3 py-1.5 flex items-center gap-2">
                  <div class="w-3 h-3 rounded bg-blue-500"></div>
                  <div class="flex-1 h-1.5 bg-slate-700 rounded-full"></div>
                  <div class="w-4 h-4 rounded-full bg-slate-700"></div>
                </div>
                <!-- Fake cards -->
                <div class="flex gap-2 flex-1">
                  <div class="w-12 bg-slate-900 rounded-lg border border-slate-800"></div>
                  <div class="flex-1 flex flex-col gap-1.5">
                    <div class="bg-slate-900 rounded-lg h-8 border border-slate-800"></div>
                    <div class="bg-slate-900 rounded-lg flex-1 border border-slate-800"></div>
                  </div>
                </div>
              </div>
              <div class="py-2.5 flex items-center justify-center gap-2 bg-slate-900">
                <MoonIcon class="w-4 h-4 text-blue-400" />
                <span class="text-xs font-black uppercase tracking-widest text-white">Oscuro</span>
                <div v-if="settingsStore.themeMode === 'dark'"
                  class="w-4 h-4 rounded-full bg-blue-500 flex items-center justify-center ml-1">
                  <CheckIcon class="w-2.5 h-2.5 text-white" />
                </div>
              </div>
            </button>
          </div>
        </section>

        <!-- ── SISTEMA ── -->
        <section class="rounded-2xl p-6 border transition-colors"
          :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200'">
          <div class="flex items-center gap-3 mb-6">
            <Settings2Icon class="w-5 h-5 text-blue-500" />
            <h3 class="text-sm font-black uppercase tracking-widest">Sistema</h3>
          </div>
          <div class="space-y-3">

            <!-- Notificaciones -->
            <div class="flex items-center justify-between p-4 rounded-xl border transition-colors"
              :class="settingsStore.isDark ? 'bg-slate-800 border-slate-700' : 'bg-slate-50 border-slate-200'">
              <div>
                <p class="text-sm font-bold">Notificaciones</p>
                <p class="text-[10px] font-bold uppercase tracking-widest mt-0.5"
                  :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
                  Alertas en tiempo real
                </p>
              </div>
              <div @click="settingsStore.toggleNotifications()"
                class="w-11 h-6 rounded-full relative cursor-pointer transition-all"
                :class="settingsStore.notificationsEnabled
                  ? 'bg-blue-500'
                  : settingsStore.isDark ? 'bg-slate-700' : 'bg-slate-300'">
                <div class="absolute top-1 w-4 h-4 bg-white rounded-full transition-all shadow-sm"
                  :class="settingsStore.notificationsEnabled ? 'left-6' : 'left-1'"></div>
              </div>
            </div>

            <!-- Idioma -->
            <div class="p-4 rounded-xl border transition-colors"
              :class="settingsStore.isDark ? 'bg-slate-800 border-slate-700' : 'bg-slate-50 border-slate-200'">
              <p class="text-sm font-bold mb-3">Idioma</p>
              <div class="flex gap-2">
                <button v-for="(label, code) in { es: 'Español', en: 'English', fr: 'Français' }"
                  :key="code"
                  @click="settingsStore.setLanguage(code)"
                  class="flex-1 py-2 rounded-lg font-black text-xs transition-all border"
                  :class="settingsStore.language === code
                    ? 'bg-blue-500 border-blue-500 text-white'
                    : settingsStore.isDark
                      ? 'bg-slate-700 border-slate-600 text-slate-400 hover:border-slate-500'
                      : 'bg-white border-slate-200 text-slate-500 hover:border-slate-300'">
                  {{ label }}
                </button>
              </div>
            </div>

          </div>
        </section>

        <!-- ── SEGURIDAD ── -->
        <section class="rounded-2xl p-6 border transition-colors"
          :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200'">
          <div class="flex items-center gap-3 mb-6">
            <ShieldIcon class="w-5 h-5 text-blue-500" />
            <h3 class="text-sm font-black uppercase tracking-widest">Seguridad</h3>
          </div>
        
          <div class="space-y-4">
            <div v-for="field in passwordFields" :key="field.key" class="relative group">
              <label class="absolute -top-2.5 left-4 px-2 text-[9px] font-black uppercase tracking-widest z-10"
                :class="settingsStore.isDark ? 'bg-slate-900 text-slate-500' : 'bg-white text-slate-400'">
                {{ field.label }}
              </label>
              <div class="relative">
                <input
                  v-model="field.value"
                  :type="field.show ? 'text' : 'password'"
                  class="w-full border-2 px-4 py-3 rounded-xl outline-none transition-all font-medium text-sm"
                  :class="settingsStore.isDark
                    ? 'bg-slate-800 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
                    : 'bg-white border-slate-200 text-slate-900 focus:border-blue-500'"
                  :placeholder="field.placeholder" />
                <button type="button" @click="field.show = !field.show"
                  class="absolute right-3 top-1/2 -translate-y-1/2 transition-colors"
                  :class="settingsStore.isDark ? 'text-slate-600 hover:text-slate-400' : 'text-slate-400 hover:text-slate-600'">
                  <EyeOffIcon v-if="field.show" class="w-4 h-4" />
                  <EyeIcon v-else class="w-4 h-4" />
                </button>
              </div>
            </div>
          
            <!-- Error / Success -->
            <p v-if="passwordError" class="text-rose-500 text-xs font-medium flex items-center gap-1">
              <AlertCircleIcon class="w-3.5 h-3.5" /> {{ passwordError }}
            </p>
            <p v-if="passwordSuccess" class="text-emerald-500 text-xs font-medium flex items-center gap-1">
              <CheckCircleIcon class="w-3.5 h-3.5" /> {{ passwordSuccess }}
            </p>
          
            <button @click="cambiarPassword" :disabled="savingPassword"
              class="w-full py-3 rounded-xl font-black text-sm uppercase tracking-widest transition-all disabled:opacity-50"
              :class="settingsStore.isDark
                ? 'bg-blue-600 hover:bg-blue-500 text-white'
                : 'bg-blue-600 hover:bg-blue-700 text-white'">
              {{ savingPassword ? 'Guardando...' : 'Actualizar Contraseña' }}
            </button>
          </div>
        </section>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import {
  ArrowLeftIcon, Settings2Icon,
  SunIcon, MoonIcon, SunMoonIcon, CheckIcon,
  ShieldIcon, EyeIcon, EyeOffIcon,
  AlertCircleIcon, CheckCircleIcon
} from 'lucide-vue-next';
import axios from 'axios';
import { useSettingsStore } from '../store/settings';
import { API_URL } from '../store/auth';

const router = useRouter();
const settingsStore = useSettingsStore();

//
const savingPassword  = ref(false);
const passwordError   = ref('');
const passwordSuccess = ref('');

const passwordFields = ref([
  { key: 'actual',    label: 'Contraseña Actual',    value: '', show: false, placeholder: '••••••••' },
  { key: 'nueva',     label: 'Contraseña Nueva',     value: '', show: false, placeholder: 'Mínimo 6 caracteres' },
  { key: 'confirmar', label: 'Confirmar Contraseña', value: '', show: false, placeholder: 'Repetí la nueva contraseña' },
]);

const cambiarPassword = async () => {
  passwordError.value   = '';
  passwordSuccess.value = '';

  const [actual, nueva, confirmar] = passwordFields.value.map(f => f.value);

  if (!actual || !nueva || !confirmar) {
    passwordError.value = 'Completá todos los campos.'; return;
  }
  if (nueva.length < 6) {
    passwordError.value = 'La contraseña nueva debe tener al menos 6 caracteres.'; return;
  }
  if (nueva !== confirmar) {
    passwordError.value = 'Las contraseñas no coinciden.'; return;
  }

  savingPassword.value = true;
  try {
    await axios.put(`${API_URL}/usuarios/cambiar-password`, {
      passwordActual:    actual,
      passwordNueva:     nueva,
      confirmarPassword: confirmar,
    });
    passwordSuccess.value = '¡Contraseña actualizada correctamente!';
    passwordFields.value.forEach(f => { f.value = ''; f.show = false; });
  } catch (err) {
    passwordError.value = err.response?.data?.message ?? 'Error al actualizar la contraseña.';
  } finally {
    savingPassword.value = false;
  }
};
</script>