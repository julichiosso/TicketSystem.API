<template>
  <div class="min-h-screen flex font-sans select-none transition-colors duration-300"
    :class="'bg-slate-50 dark:bg-slate-950 text-slate-900 dark:bg-slate-950 dark:text-white'">
    <Sidebar />
    <!-- Main -->
    <main class="flex-1 min-w-0 h-screen overflow-y-auto page-fade-in custom-scrollbar">
      <header class="h-20 flex items-center px-10 sticky top-0 z-30 border-b transition-colors"
        :class="'bg-white dark:bg-slate-900 border-slate-100 dark:bg-slate-950 dark:border-slate-800'">
        <h2 class="text-2xl font-black tracking-tight uppercase italic opacity-90">Mi Perfil</h2>
      </header>
      <div class="max-w-3xl mx-auto py-16 px-10 space-y-8 page-fade-in">
        <!-- Avatar Card -->
        <div class="rounded-3xl p-10 flex flex-col sm:flex-row items-center gap-10 border transition-colors"
          :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 shadow-sm dark:bg-slate-900 dark:border-slate-800'">
          <div :class="`relative w-32 h-32 rounded-3xl bg-gradient-to-br ${settingsStore.themeClasses} flex items-center justify-center text-5xl font-black text-white shadow-md select-none`">
            {{ authStore.user?.nombre?.[0]?.toUpperCase() }}
            <div class="absolute -bottom-2 -right-2 w-6 h-6 bg-emerald-500 rounded-full border-4"
              :class="'border-white dark:border-slate-900'"></div>
          </div>
          <div class="text-center sm:text-left">
            <h1 class="text-4xl font-black tracking-tighter">{{ authStore.user?.nombre }}</h1>
            <p class="text-sm mt-1" :class="'text-slate-500 dark:text-slate-400'">
              {{ authStore.user?.email }}
            </p>
            <div class="mt-4 flex gap-3 justify-center sm:justify-start flex-wrap">
              <span class="text-xs font-black uppercase tracking-widest px-4 py-1.5 rounded-full border"
                :class="'bg-blue-50 text-blue-600 border-blue-200 dark:bg-blue-500/10 dark:text-blue-400 dark:border-blue-500/20'">
                {{ authStore.user?.rol }}
              </span>
              <span class="text-xs font-black uppercase tracking-widest bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 px-4 py-1.5 rounded-full">
                Activo
              </span>
            </div>
          </div>
        </div>
        <!-- Info Cards -->
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-6">
          <div class="rounded-3xl p-8 border transition-colors"
            :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 shadow-sm dark:bg-slate-900 dark:border-slate-800'">
            <h3 class="text-[10px] font-black uppercase tracking-widest mb-4"
              :class="'text-slate-400 dark:text-slate-500'">
              Información Personal
            </h3>
            <div class="space-y-4">
              <div v-for="(val, label) in infoItems" :key="label">
                <p class="text-[9px] font-bold uppercase tracking-widest mb-1"
                  :class="'text-slate-400 dark:text-slate-600'">{{ label }}</p>
                <p class="font-bold">{{ val }}</p>
              </div>
            </div>
          </div>
          <div class="rounded-3xl p-8 border transition-colors"
            :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 shadow-sm dark:bg-slate-900 dark:border-slate-800'">
            <h3 class="text-[10px] font-black uppercase tracking-widest mb-4"
              :class="'text-slate-400 dark:text-slate-500'">
              Acceso Rápido
            </h3>
            <div class="space-y-3">
              <router-link
                :to="authStore.isOperador ? '/admin' : '/dashboard'"
                class="flex items-center gap-4 p-4 rounded-2xl transition-all group"
                :class="'hover:bg-slate-50 dark:hover:bg-slate-800'">
                <LayoutDashboardIcon class="w-5 h-5 text-blue-500 group-hover:scale-110 transition-transform" />
                <span class="text-sm font-bold">Ir al Dashboard</span>
              </router-link>
              <router-link to="/settings"
                class="flex items-center gap-4 p-4 rounded-2xl transition-all group"
                :class="'hover:bg-slate-50 dark:hover:bg-slate-800'">
                <SettingsIcon class="w-5 h-5 text-blue-500 group-hover:scale-110 transition-transform" />
                <span class="text-sm font-bold">Configuración del Sistema</span>
              </router-link>
            </div>
          </div>
        </div>
        <!-- Logout -->
        <button @click="handleLogout"
          class="w-full py-5 font-black text-sm uppercase tracking-widest rounded-3xl border transition-all flex items-center justify-center gap-3"
          :class="'bg-rose-50 hover:bg-rose-100 text-rose-500 border-rose-200 dark:bg-rose-500/5 dark:hover:bg-rose-500/10 dark:text-rose-400 dark:border-rose-500/10'">
          <LogOutIcon class="w-5 h-5" />
          Cerrar Sesión
        </button>
      </div>
    </main>
  </div>
</template>
<script setup>
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { LayoutDashboardIcon, SettingsIcon, LogOutIcon } from 'lucide-vue-next';
import { useAuthStore } from '../store/auth';
import { useSettingsStore } from '../store/settings';
import Sidebar from '../components/Sidebar.vue';
const authStore = useAuthStore();
const settingsStore = useSettingsStore();
const router = useRouter();
const infoItems = computed(() => ({
  'Nombre': authStore.user?.nombre,
  'Email': authStore.user?.email,
  'Rol': authStore.user?.rol,
}));
const handleLogout = async () => {
  await authStore.logout();
  router.push('/');
};
</script>