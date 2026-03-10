<template>
  <aside
    class="flex flex-col transition-all duration-500 sticky top-0 h-screen z-50 shadow-sm overflow-hidden border-r"
    :class="[
      isExpanded ? 'w-64' : 'w-20',
      settingsStore.isDark
        ? 'bg-slate-950 border-slate-800'
        : 'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700'
    ]"
    @mouseenter="isExpanded = true"
    @mouseleave="isExpanded = false"
  >
    <!-- Logo Section -->
    <div class="flex items-center h-20 flex-shrink-0 border-b"
      :class="'border-slate-100 dark:border-slate-800'">
      <div class="w-20 flex-shrink-0 flex items-center justify-center">
        <!-- Logo mejorado -->
        <div :class="`w-10 h-10 bg-gradient-to-tr ${settingsStore.themeClasses} rounded-2xl flex items-center justify-center shadow-lg transition-all active:scale-95`">
          <TicketIcon class="text-white w-5 h-5" />
        </div>
      </div>
      <div
        class="transition-all duration-500 overflow-hidden whitespace-nowrap"
        :class="isExpanded ? 'opacity-100 w-auto' : 'opacity-0 w-0'"
      >
        <p class="text-base font-black tracking-tight leading-none"
          :class="'text-slate-900 dark:text-white'">
          TicketSystem
        </p>
        <p class="text-[10px] font-medium tracking-widest uppercase mt-0.5"
          :class="'text-slate-400 dark:text-slate-500'">
          Soporte
        </p>
      </div>
    </div>
    <!-- Navigation -->
    <nav class="flex-1 py-4 space-y-1 overflow-y-auto overflow-x-hidden px-2">
      <div v-for="item in navItems" :key="item.to">
        <router-link
          v-if="item.show"
          :to="item.to"
          class="flex items-center py-3 rounded-xl transition-all duration-200 group/link relative overflow-hidden"
          :class="isActive(item.to)
            ? `bg-gradient-to-r ${settingsStore.themeClasses} text-white shadow-md`
            : settingsStore.isDark
              ? 'text-slate-400 hover:text-white hover:bg-slate-800'
              : 'text-slate-500 hover:text-slate-900 dark:text-white hover:bg-slate-100 dark:hover:bg-slate-800'"
        >
          <div class="w-16 flex-shrink-0 flex items-center justify-center">
            <component :is="item.icon" class="w-5 h-5 transition-transform duration-200 group-hover/link:scale-110" />
          </div>
          <span
            class="text-sm font-medium tracking-wide transition-all duration-500 whitespace-nowrap"
            :class="isExpanded ? 'opacity-100' : 'opacity-0 w-0'"
          >
            {{ item.label }}
          </span>
        </router-link>
      </div>
    </nav>
    <!-- User Section -->
    <div class="py-3 flex-shrink-0 flex flex-col gap-1 px-2 border-t"
      :class="'border-slate-100 dark:border-slate-800'">
      <router-link
        to="/profile"
        class="flex items-center py-2 rounded-xl transition-all duration-200 group/link"
        :class="isActive('/profile')
          ? settingsStore.isDark ? 'bg-slate-800' : 'bg-slate-100'
          : settingsStore.isDark
            ? 'hover:bg-slate-800 text-slate-400 hover:text-white'
            : 'hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-500 hover:text-slate-900 dark:text-white'"
      >
        <div class="w-16 flex-shrink-0 flex items-center justify-center">
          <div :class="`w-9 h-9 rounded-xl flex items-center justify-center overflow-hidden shadow-sm bg-gradient-to-tr ${settingsStore.themeClasses}`">
            <span class="text-xs font-black text-white">
              {{ (authStore.user?.nombre?.[0] ?? 'U').toUpperCase() }}
            </span>
          </div>
        </div>
        <div
          class="flex flex-col overflow-hidden transition-all duration-500 whitespace-nowrap"
          :class="isExpanded ? 'opacity-100 w-auto' : 'opacity-0 w-0'"
        >
          <span class="text-xs font-bold truncate"
            :class="'text-slate-900 dark:text-white'">
            {{ authStore.user?.nombre }}
          </span>
          <span class="text-[10px] font-medium truncate"
            :class="'text-slate-400 dark:text-slate-500'">
            {{ authStore.user?.rol }}
          </span>
        </div>
      </router-link>
      <button
        @click="handleLogout"
        class="w-full flex items-center py-3 rounded-xl transition-all duration-200 active:scale-95 group/logout"
        :class="'text-slate-400 hover:text-rose-500 hover:bg-rose-50 dark:text-slate-500 dark:hover:text-rose-400 dark:hover:bg-rose-500/10'"
      >
        <div class="w-16 flex-shrink-0 flex items-center justify-center">
          <LogOutIcon class="w-5 h-5 transition-transform group-hover/logout:-translate-x-1" />
        </div>
        <span
          class="font-medium text-sm transition-all duration-500 whitespace-nowrap"
          :class="isExpanded ? 'opacity-100 w-auto' : 'opacity-0 w-0'"
        >
          Cerrar Sesión
        </span>
      </button>
    </div>
  </aside>
</template>
<script setup>
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import {
  TicketIcon,
  LayoutDashboardIcon,
  SettingsIcon,
  LogOutIcon,
  UserIcon,
  ShieldCheckIcon,
  HardHatIcon
} from 'lucide-vue-next';
import { useAuthStore } from '../store/auth';
import { useSettingsStore } from '../store/settings';
const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const settingsStore = useSettingsStore();
const isExpanded = ref(false);
const navItems = computed(() => [
  { label: 'Panel Principal', to: '/dashboard', icon: LayoutDashboardIcon, show: true },
  { label: 'Admin Suite',     to: '/admin',     icon: ShieldCheckIcon,    show: authStore.isAdmin },
  { label: 'Operador',        to: '/operator',  icon: HardHatIcon,        show: authStore.isOperador },
  { label: 'Mi Perfil',       to: '/profile',   icon: UserIcon,           show: true },
  { label: 'Ajustes',         to: '/settings',  icon: SettingsIcon,       show: true },
]);
const isActive = (path) => route.path === path;
const handleLogout = async () => {
  await authStore.logout();
  router.push('/');
};
</script>