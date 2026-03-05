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

        <!-- ── COLOR DE ACENTO ── -->
        <section class="rounded-2xl p-6 border transition-colors"
          :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200'">
          <div class="flex items-center gap-3 mb-6">
            <PaletteIcon class="w-5 h-5 text-blue-500" />
            <h3 class="text-sm font-black uppercase tracking-widest">Color de Acento</h3>
          </div>

          <div class="flex gap-3">
            <button v-for="c in colorOptions" :key="c.key"
              @click="settingsStore.setThemeColor(c.key)"
              class="flex-1 h-12 rounded-xl border-2 transition-all relative overflow-hidden"
              :class="settingsStore.themeColor === c.key
                ? 'border-white scale-105 shadow-lg'
                : 'border-transparent opacity-60 hover:opacity-90 hover:scale-105'"
              :style="`background: linear-gradient(135deg, ${c.from}, ${c.to})`">
              <div v-if="settingsStore.themeColor === c.key"
                class="absolute inset-0 flex items-center justify-center">
                <CheckIcon class="w-4 h-4 text-white drop-shadow" />
              </div>
              <span class="absolute bottom-1 w-full text-center text-[8px] font-black uppercase tracking-widest text-white/70">
                {{ c.label }}
              </span>
            </button>
          </div>
        </section>

        <!-- ── DENSIDAD ── -->
        <section class="rounded-2xl p-6 border transition-colors"
          :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200'">
          <div class="flex items-center gap-3 mb-6">
            <LayersIcon class="w-5 h-5 text-blue-500" />
            <h3 class="text-sm font-black uppercase tracking-widest">Densidad de Interfaz</h3>
          </div>
          <div class="flex gap-3">
            <button v-for="d in densityOptions" :key="d.key"
              @click="settingsStore.setUIDensity(d.key)"
              class="flex-1 py-4 rounded-xl font-black uppercase tracking-widest text-xs transition-all border-2 flex flex-col items-center gap-2"
              :class="settingsStore.uiDensity === d.key
                ? 'bg-blue-500 border-blue-500 text-white shadow-md shadow-blue-500/20'
                : settingsStore.isDark
                  ? 'bg-slate-800 border-slate-700 text-slate-400 hover:border-slate-600'
                  : 'bg-white border-slate-200 text-slate-500 hover:border-slate-300'">
              <component :is="d.icon" class="w-4 h-4" />
              {{ d.label }}
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

      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';
import {
  ArrowLeftIcon, PaletteIcon, LayersIcon, Settings2Icon,
  SunIcon, MoonIcon, SunMoonIcon, CheckIcon,
  AlignJustifyIcon, AlignCenterIcon
} from 'lucide-vue-next';
import { useSettingsStore } from '../store/settings';

const router = useRouter();
const settingsStore = useSettingsStore();

const colorOptions = [
  { key: 'blue',    label: 'Azul',      from: '#3b82f6', to: '#1d4ed8' },
  { key: 'indigo',  label: 'Índigo',    from: '#6366f1', to: '#4338ca' },
 // { key: 'emerald', label: 'Esmeralda', from: '#10b981', to: '#047857' },
 // { key: 'rose',    label: 'Rosa',      from: '#f43f5e', to: '#be123c' },
  { key: 'violet',  label: 'Violeta',   from: '#8b5cf6', to: '#6d28d9' },
];




const densityOptions = [
  { key: 'comfortable', label: 'Cómoda',   icon: AlignJustifyIcon },
  { key: 'compact',     label: 'Compacta', icon: AlignCenterIcon  },
];
</script>