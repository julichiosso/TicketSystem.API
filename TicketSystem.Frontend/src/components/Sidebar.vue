<template>
  <!-- Desktop Sidebar -->
  <aside
    class="hidden md:flex flex-col transition-all duration-500 sticky top-0 h-screen z-50 shadow-sm overflow-hidden border-r"
    :class="[
      isExpanded ? 'w-64' : 'w-20',
      settingsStore.isDark
        ? 'bg-slate-950 border-slate-800'
        : 'bg-white border-slate-200'
    ]"
    @mouseenter="isExpanded = true"
    @mouseleave="isExpanded = false"
  >
    
    <div class="flex items-center h-20 flex-shrink-0 border-b"
      :class="'border-slate-100 dark:border-slate-800'">
      <div class="w-20 flex-shrink-0 flex items-center justify-center">
        
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
          Support
        </p>
      </div>
    </div>
    
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
              : 'text-slate-500 hover:text-slate-900 hover:bg-slate-100'"
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
    
    <div class="py-3 flex-shrink-0 flex flex-col gap-1 px-2 border-t"
      :class="'border-slate-100 dark:border-slate-800'">
      <router-link
        to="/profile"
        class="flex items-center py-2 rounded-xl transition-all duration-200 group/link"
        :class="isActive('/profile')
          ? settingsStore.isDark ? 'bg-slate-800' : 'bg-slate-100'
          : settingsStore.isDark
            ? 'hover:bg-slate-800 text-slate-400 hover:text-white'
            : 'hover:bg-slate-100 text-slate-500 hover:text-slate-900'"
      >
        <div class="w-16 flex-shrink-0 flex items-center justify-center">
          <div :class="`w-9 h-9 rounded-xl flex items-center justify-center overflow-hidden shadow-sm bg-gradient-to-tr ${settingsStore.themeClasses}`">
            <span class="text-xs font-black text-white">
              {{ (authStore.user?.name?.[0] ?? 'U').toUpperCase() }}
            </span>
          </div>
        </div>
        <div
          class="flex flex-col overflow-hidden transition-all duration-500 whitespace-nowrap"
          :class="isExpanded ? 'opacity-100 w-auto' : 'opacity-0 w-0'"
        >
          <span class="text-xs font-bold truncate"
            :class="'text-slate-900 dark:text-white'">
            {{ authStore.user?.name }}
          </span>
          <span class="text-[10px] font-medium truncate"
            :class="'text-slate-400 dark:text-slate-500'">
            {{ authStore.user?.role }}
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
          Log Out
        </span>
      </button>
    </div>
  </aside>

  <!-- Mobile Floating Menu Button -->
  <button
    @click="isMobileMenuOpen = true"
    class="md:hidden fixed top-4 left-4 z-[60] p-3 rounded-2xl shadow-lg border transition-all active:scale-95"
    :class="settingsStore.isDark
      ? 'bg-slate-900 border-slate-800 text-white'
      : 'bg-white border-slate-200 text-slate-900'"
  >
    <MenuIcon class="w-5 h-5" />
  </button>

  <!-- Mobile Menu Drawer Overlay -->
  <Teleport to="body">
    <div v-if="isMobileMenuOpen" class="md:hidden fixed inset-0 z-[70]">
      <!-- Backdrop -->
      <div
        class="absolute inset-0 bg-slate-950/40 backdrop-blur-sm transition-opacity"
        @click="isMobileMenuOpen = false"
      ></div>
      
      <!-- Drawer -->
      <aside
        class="absolute inset-y-0 left-0 w-72 flex flex-col shadow-2xl transition-transform duration-300"
        :class="settingsStore.isDark ? 'bg-slate-950 border-r border-slate-800' : 'bg-white'"
      >
        <div class="flex items-center justify-between h-20 px-6 border-b"
          :class="'border-slate-100 dark:border-slate-800'">
          <div class="flex items-center gap-3">
            <div :class="`w-9 h-9 bg-gradient-to-tr ${settingsStore.themeClasses} rounded-xl flex items-center justify-center shadow-lg`">
              <TicketIcon class="text-white w-4 h-4" />
            </div>
            <div>
              <p class="text-sm font-black tracking-tight" :class="'text-slate-900 dark:text-white'">TicketSystem</p>
              <p class="text-[9px] font-bold uppercase tracking-widest text-slate-400">Platform</p>
            </div>
          </div>
          <button @click="isMobileMenuOpen = false" class="p-2 rounded-lg text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800">
            <XIcon class="w-5 h-5" />
          </button>
        </div>

        <nav class="flex-1 py-6 px-4 space-y-1 overflow-y-auto">
          <router-link
            v-for="item in navItems.filter(i => i.show)"
            :key="item.to"
            :to="item.to"
            @click="isMobileMenuOpen = false"
            class="flex items-center gap-4 py-3.5 px-4 rounded-xl transition-all"
            :class="isActive(item.to)
              ? `bg-gradient-to-r ${settingsStore.themeClasses} text-white shadow-md font-bold`
              : 'text-slate-500 dark:text-slate-400'"
          >
            <component :is="item.icon" class="w-5 h-5" />
            <span class="text-sm tracking-wide">{{ item.label }}</span>
          </router-link>
        </nav>

        <div class="p-4 border-t" :class="'border-slate-100 dark:border-slate-800'">
          <router-link
            to="/profile"
            @click="isMobileMenuOpen = false"
            class="flex items-center gap-3 p-3 rounded-2xl mb-2"
            :class="settingsStore.isDark ? 'bg-slate-900' : 'bg-slate-50'"
          >
            <div :class="`w-10 h-10 rounded-xl bg-gradient-to-tr ${settingsStore.themeClasses} flex items-center justify-center text-white font-black` text-sm shadow-sm">
              {{ (authStore.user?.name?.[0] ?? 'U').toUpperCase() }}
            </div>
            <div class="flex flex-col min-w-0">
              <span class="text-xs font-bold truncate dark:text-white">{{ authStore.user?.name }}</span>
              <span class="text-[10px] text-slate-400 truncate">{{ authStore.user?.role }}</span>
            </div>
          </router-link>
          <button
            @click="handleLogout"
            class="w-full flex items-center gap-3 py-3 px-4 rounded-xl text-rose-500 font-bold text-sm tracking-wide"
          >
            <LogOutIcon class="w-5 h-5" />
            Log Out
          </button>
        </div>
      </aside>
    </div>
  </Teleport>
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
  HardHatIcon,
  MenuIcon,
  XIcon
} from 'lucide-vue-next';
import { useAuthStore } from '../store/auth';
import { useSettingsStore } from '../store/settings';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const settingsStore = useSettingsStore();

const isExpanded = ref(false);
const isMobileMenuOpen = ref(false);

const navItems = computed(() => [
  { label: 'Dashboard',   to: '/dashboard', icon: LayoutDashboardIcon, show: true },
  { label: 'Admin Suite', to: '/admin',     icon: ShieldCheckIcon,    show: authStore.isAdmin },
  { label: 'Operator',    to: '/operator',  icon: HardHatIcon,        show: authStore.isOperator },
  { label: 'My Profile',  to: '/profile',   icon: UserIcon,           show: true },
  { label: 'Settings',    to: '/settings',  icon: SettingsIcon,       show: true },
]);

const isActive = (path) => route.path === path;

const handleLogout = async () => {
  await authStore.logout();
  router.push('/');
};
</script>
