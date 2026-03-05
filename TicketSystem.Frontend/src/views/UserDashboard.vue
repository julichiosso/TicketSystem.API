<template>
  <div class="min-h-screen flex font-sans select-none transition-colors duration-300"
    :class="settingsStore.isDark ? 'bg-slate-950 text-white' : 'bg-slate-50 text-slate-900'">

    <Sidebar />

    <!-- Main Content -->
    <main class="flex-1 min-w-0 h-screen overflow-y-auto page-fade-in custom-scrollbar">

      <!-- Header -->
      <header class="h-20 flex items-center justify-between px-10 sticky top-0 z-30 border-b transition-colors"
        :class="settingsStore.isDark ? 'bg-slate-950 border-slate-800' : 'bg-white border-slate-100'">
        <div>
          <h2 class="text-xl font-semibold tracking-tight">Mis Tickets</h2>
          <p class="text-[10px] font-bold uppercase tracking-[0.2em]"
            :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
            Soporte y Seguimiento
          </p>
        </div>

        <div class="flex items-center gap-4">
          <button @click="fetchTickets"
            class="p-2.5 rounded-xl transition-all active:rotate-180 duration-700"
            :class="settingsStore.isDark ? 'hover:bg-slate-800 text-slate-400' : 'hover:bg-slate-100 text-slate-500'"
            title="Presiona R para refrescar">
            <RefreshCcwIcon class="w-5 h-5" />
          </button>

          <div class="h-8 w-px" :class="settingsStore.isDark ? 'bg-slate-700' : 'bg-slate-200'"></div>

          <!-- Profile menu -->
          <div class="relative profile-menu-container">
            <button @click.stop="showProfileMenu = !showProfileMenu"
              class="flex items-center gap-4 group cursor-pointer transition-all">
              <div class="text-right hidden sm:block">
                <div class="text-xs font-black uppercase tracking-wider group-hover:text-blue-400 transition-colors">
                  {{ authStore.user?.nombre }}
                </div>
                <div class="text-[9px] font-bold uppercase tracking-widest"
                  :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
                  {{ authStore.user?.rol }}
                </div>
              </div>
              <div :class="`w-10 h-10 rounded-2xl bg-gradient-to-br ${settingsStore.themeClasses} flex items-center justify-center font-black text-white shadow-sm`">
                {{ authStore.user?.nombre?.[0] }}
              </div>
            </button>

            <div v-if="showProfileMenu"
              class="absolute right-0 mt-4 w-56 rounded-3xl shadow-lg py-3 z-50 animate-in slide-in-from-top-2 duration-300 border"
              :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200'">
              <router-link to="/profile"
                class="flex items-center gap-3 px-6 py-3 transition-all"
                :class="settingsStore.isDark ? 'text-slate-400 hover:text-white hover:bg-slate-800' : 'text-slate-500 hover:text-slate-900 hover:bg-slate-50'">
                <UserIcon class="w-4 h-4" />
                <span class="text-xs font-bold uppercase tracking-widest">Mi Perfil</span>
              </router-link>
              <router-link to="/settings"
                class="flex items-center gap-3 px-6 py-3 transition-all"
                :class="settingsStore.isDark ? 'text-slate-400 hover:text-white hover:bg-slate-800' : 'text-slate-500 hover:text-slate-900 hover:bg-slate-50'">
                <SettingsIcon class="w-4 h-4" />
                <span class="text-xs font-bold uppercase tracking-widest">Ajustes</span>
              </router-link>
              <div class="h-px my-2 mx-6" :class="settingsStore.isDark ? 'bg-slate-800' : 'bg-slate-100'"></div>
              <button @click="handleLogout"
                class="w-full flex items-center gap-3 px-6 py-3 text-rose-500 hover:bg-rose-500/10 transition-all">
                <LogOutIcon class="w-4 h-4" />
                <span class="text-xs font-bold uppercase tracking-widest">Cerrar Sesión</span>
              </button>
            </div>
          </div>
        </div>
      </header>

      <div class="max-w-[1600px] mx-auto transition-all duration-500" :style="{ padding: 'var(--density-p, 2.5rem)' }">

        <!-- Welcome -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div class="rounded-2xl p-6 col-span-1 md:col-span-2 relative overflow-hidden border transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-100 shadow-sm'">
            <div class="flex items-center justify-between relative z-10">
              <div>
                <h2 class="text-xl font-bold mb-1">¡Hola, {{ authStore.user?.nombre }}!</h2>
                <p class="text-sm" :class="settingsStore.isDark ? 'text-slate-400' : 'text-slate-500'">
                  Gestiona y da seguimiento a tus tickets de soporte
                </p>
              </div>
              <div class="text-5xl opacity-10">🎟️</div>
            </div>
          </div>
          <div class="rounded-2xl p-6 relative overflow-hidden border transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-100 shadow-sm'">
            <p class="text-[10px] font-semibold uppercase tracking-wider mb-2"
              :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">Tiempo Promedio</p>
            <h3 class="text-xl font-bold text-blue-500 mb-1">~2h</h3>
            <p class="text-xs" :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">Respuesta del equipo</p>
          </div>
        </div>

        <!-- Stats -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-8 mb-16">
          <div v-for="stat in statCards" :key="stat.label"
            class="rounded-2xl p-8 relative overflow-hidden group hover:scale-[1.01] transition-all border"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-100 shadow-sm'">
            <div class="relative z-10">
              <p class="text-[9px] font-bold uppercase tracking-widest mb-1"
                :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">{{ stat.label }}</p>
              <h4 class="text-3xl font-bold mb-2">{{ stat.value }}</h4>
              <p class="text-xs" :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">{{ stat.desc }}</p>
            </div>
            <component :is="stat.icon"
              class="absolute -right-4 -bottom-4 w-24 h-24 opacity-5 group-hover:scale-110 transition-transform duration-700" />
          </div>
        </div>

        <!-- Toolbar -->
        <div class="flex flex-col sm:flex-row items-center justify-between mb-8 px-2 gap-4">
          <div class="flex items-center gap-6">
            <h3 class="text-base font-bold flex items-center gap-3">
              <div :class="`w-1 h-5 bg-gradient-to-b ${settingsStore.themeClasses} rounded-full`"></div>
              LISTADO DE TICKETS
            </h3>
            <button @click="showModal = true"
              class="group flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2.5 rounded-lg font-medium transition-colors shadow-sm">
              <PlusIcon class="w-4 h-4 group-hover:rotate-90 transition-transform" />
              <span class="text-sm hidden sm:inline">Nuevo Ticket</span>
            </button>
          </div>

          <div class="relative w-full max-w-xs">
            <input v-model="searchQuery" type="text" placeholder="Buscar tickets..."
              class="w-full border px-4 py-2 rounded-xl text-sm outline-none transition"
              :class="settingsStore.isDark
                ? 'bg-slate-900 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
                : 'bg-white border-slate-200 text-slate-900 placeholder-slate-400 focus:border-blue-500'" />
            <SearchIcon class="w-4 h-4 absolute right-3 top-1/2 -translate-y-1/2"
              :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'" />
          </div>

          <div class="text-[10px] font-bold uppercase tracking-widest px-4 py-2 rounded-full border"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-700 text-slate-500' : 'bg-white border-slate-200 text-slate-500'">
            Total: {{ filteredTickets.length }} tickets
          </div>
        </div>

        <!-- Loading -->
        <div v-if="isLoading" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCardSkeleton v-for="n in 8" :key="n" />
        </div>

        <!-- Tickets -->
        <div v-else-if="filteredTickets.length > 0"
          class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCard v-for="ticket in filteredTickets" :key="ticket.id"
            :ticket="ticket" @click="openTicketDetail(ticket)" class="cursor-pointer" />
        </div>

        <!-- Empty -->
        <div v-else
          class="flex flex-col items-center justify-center py-32 border-2 border-dashed rounded-[3rem] group transition-all duration-700"
          :class="settingsStore.isDark ? 'border-slate-800 bg-slate-900/50' : 'border-slate-200 bg-white'">
          <div class="w-24 h-24 rounded-full flex items-center justify-center mb-8 border group-hover:scale-110 transition-all duration-700"
            :class="settingsStore.isDark ? 'bg-slate-800 border-slate-700' : 'bg-white border-slate-200'">
            <InboxIcon class="w-10 h-10 group-hover:text-blue-500 transition-colors"
              :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'" />
          </div>
          <h3 class="text-xl font-black mb-2 uppercase italic tracking-tight">
            {{ tickets.length === 0 ? 'Tu buzón está vacío' : 'Sin resultados' }}
          </h3>
          <p class="text-sm max-w-sm text-center mb-10 font-medium leading-relaxed"
            :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
            <template v-if="tickets.length === 0">
              No tenés tickets activos. Si necesitás soporte, creá uno nuevo.
            </template>
            <template v-else>
              No se encontraron tickets que coincidan con "{{ searchQuery }}".
            </template>
          </p>
          <button v-if="tickets.length === 0" @click="showModal = true"
            class="group flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white font-medium px-6 py-3 rounded-lg shadow-sm transition-colors">
            <PlusIcon class="w-5 h-5 group-hover:rotate-90 transition-transform duration-300" />
            Crear mi primer ticket
          </button>
        </div>
      </div>
    </main>

    <!-- Create Ticket Modal -->
    <div v-if="showModal"
      class="fixed inset-0 z-[100] flex items-center justify-center p-6 backdrop-blur-sm transition-all"
      :class="settingsStore.isDark ? 'bg-slate-950/70' : 'bg-slate-900/30'">
      <div class="rounded-[3rem] w-full max-w-xl shadow-xl relative border transition-colors"
        :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200'">
        <div class="p-10">
          <div class="flex justify-between items-start mb-8">
            <div>
              <h3 class="text-2xl font-bold">Nuevo Ticket</h3>
              <p class="text-sm mt-1" :class="settingsStore.isDark ? 'text-slate-400' : 'text-slate-500'">
                Ingresa los detalles de tu problema
              </p>
            </div>
            <button @click="showModal = false"
              class="p-2 rounded-full transition-all"
              :class="settingsStore.isDark ? 'hover:bg-slate-800 text-slate-400' : 'hover:bg-slate-100 text-slate-400'">
              <XIcon class="w-5 h-5" />
            </button>
          </div>

          <form @submit.prevent="createTicket" class="space-y-6">
            <div class="relative group">
              <label class="absolute -top-2.5 left-4 px-2 text-[9px] font-black uppercase tracking-widest z-10"
                :class="settingsStore.isDark ? 'bg-slate-900 text-slate-500' : 'bg-white text-slate-500'">
                Asunto del Problema
              </label>
              <input v-model="newTicket.title" @input="formErrors.title = ''" maxlength="100"
                class="w-full border-2 px-6 py-4 rounded-2xl outline-none transition-all font-medium"
                :class="settingsStore.isDark
                  ? 'bg-slate-800 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
                  : 'bg-white border-slate-200 text-slate-900 placeholder-slate-400 focus:border-blue-500'"
                placeholder="Ej: Error al procesar pago" />
              <p v-if="formErrors.title" class="text-rose-500 text-xs font-medium mt-1.5 flex items-center gap-1">
                <AlertCircleIcon class="w-3.5 h-3.5" /> {{ formErrors.title }}
              </p>
              <div class="text-[8px] mt-1" :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
                {{ newTicket.title.length }}/100
              </div>
            </div>

            <div class="relative group">
              <label class="absolute -top-2.5 left-4 px-2 text-[9px] font-black uppercase tracking-widest z-10"
                :class="settingsStore.isDark ? 'bg-slate-900 text-slate-500' : 'bg-white text-slate-500'">
                Descripción Detallada
              </label>
              <textarea v-model="newTicket.description" @input="formErrors.description = ''" rows="4" maxlength="1000"
                class="w-full border-2 px-6 py-4 rounded-2xl outline-none transition-all font-medium"
                :class="settingsStore.isDark
                  ? 'bg-slate-800 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
                  : 'bg-white border-slate-200 text-slate-900 placeholder-slate-400 focus:border-blue-500'"
                placeholder="¿Qué está fallando exactamente?"></textarea>
              <p v-if="formErrors.description" class="text-rose-500 text-xs font-medium mt-1.5 flex items-center gap-1">
                <AlertCircleIcon class="w-3.5 h-3.5" /> {{ formErrors.description }}
              </p>
            </div>

            <!-- Prioridad -->
            <div class="relative w-1/2" v-click-outside="() => isPriorityDropdownOpen = false">
              <label class="absolute -top-2.5 left-4 px-2 text-[9px] font-black uppercase tracking-widest z-10"
                :class="settingsStore.isDark ? 'bg-slate-900 text-slate-500' : 'bg-white text-slate-500'">
                Prioridad
              </label>
              <div @click="isPriorityDropdownOpen = !isPriorityDropdownOpen"
                class="w-full border-2 px-6 py-4 rounded-2xl cursor-pointer outline-none transition-all font-bold flex justify-between items-center"
                :class="settingsStore.isDark
                  ? 'bg-slate-800 border-slate-700 text-white hover:border-blue-500'
                  : 'bg-white border-slate-200 text-slate-900 hover:border-blue-400'">
                <div class="flex items-center gap-3">
                  <component :is="priorityOptions[newTicket.priority].icon"
                    :class="priorityOptions[newTicket.priority].colorClass" class="w-5 h-5" />
                  <span>{{ priorityOptions[newTicket.priority].label }}</span>
                </div>
                <ChevronDownIcon class="w-5 h-5 transition-transform"
                  :class="[isPriorityDropdownOpen ? 'rotate-180' : '', settingsStore.isDark ? 'text-slate-500' : 'text-slate-400']" />
              </div>
              <div v-if="isPriorityDropdownOpen"
                class="absolute bottom-full left-0 mb-2 w-full rounded-2xl shadow-xl z-50 overflow-hidden border"
                :class="settingsStore.isDark ? 'bg-slate-800 border-slate-700' : 'bg-white border-slate-200'">
                <button v-for="(option, key) in priorityOptions" :key="key" type="button"
                  @click="newTicket.priority = key; isPriorityDropdownOpen = false"
                  class="w-full px-6 py-4 flex items-center gap-3 transition-colors text-left"
                  :class="[
                    newTicket.priority === key
                      ? settingsStore.isDark ? 'bg-blue-500/20' : 'bg-blue-50'
                      : settingsStore.isDark ? 'hover:bg-slate-700' : 'hover:bg-slate-50'
                  ]">
                  <component :is="option.icon" :class="option.colorClass" class="w-5 h-5" />
                  <span class="font-bold">{{ option.label }}</span>
                </button>
              </div>
            </div>

            <div class="flex gap-4 pt-4 border-t"
              :class="settingsStore.isDark ? 'border-slate-800' : 'border-slate-100'">
              <button @click="showModal = false" type="button"
                class="px-6 py-2.5 border font-medium rounded-lg transition-colors"
                :class="settingsStore.isDark
                  ? 'bg-slate-800 border-slate-700 text-slate-300 hover:bg-slate-700'
                  : 'bg-white border-slate-300 text-slate-700 hover:bg-slate-50'">
                Cancelar
              </button>
              <button type="submit" :disabled="creatingTicket"
                class="flex-1 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg shadow-sm transition-colors flex items-center justify-center gap-2 disabled:opacity-50">
                {{ creatingTicket ? 'Enviando...' : 'Enviar Solicitud' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Ticket Detail Modal -->
    <TicketDetailModal v-if="selectedTicket" :ticket="selectedTicket"
      @close="selectedTicket = null; ticketsStore.clearSelection()" />
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, reactive, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import {
  InboxIcon, TimerIcon, CheckCircleIcon, RefreshCcwIcon, XIcon,
  PlusIcon, UserIcon, SettingsIcon, SearchIcon, ChevronDownIcon,
  CheckCircle2Icon, AlertTriangleIcon, AlertOctagonIcon, AlertCircleIcon,
  LogOutIcon
} from 'lucide-vue-next';
import { useAuthStore, API_URL } from '../store/auth';
import { useNotificationStore } from '../store/notifications';
import { useSettingsStore } from '../store/settings';
import { useTicketsStore } from '../store/tickets';
import TicketCard from '../components/TicketCard.vue';
import TicketCardSkeleton from '../components/TicketCardSkeleton.vue';
import TicketDetailModal from '../components/TicketDetailModal.vue';
import Sidebar from '../components/Sidebar.vue';

const authStore = useAuthStore();
const notificationStore = useNotificationStore();
const settingsStore = useSettingsStore();
const ticketsStore = useTicketsStore();
const router = useRouter();

const tickets = ref([]);
const isLoading = ref(false);
const searchQuery = ref('');
const showModal = ref(false);
const showProfileMenu = ref(false);
const selectedTicket = ref(null);

const stats = reactive({ open: 0, processing: 0, resolved: 0 });
const statCards = computed(() => [
  { label: 'Pendientes',  value: stats.open,       desc: 'En espera de respuesta', icon: InboxIcon },
  { label: 'En Proceso',  value: stats.processing, desc: 'Siendo atendidos',        icon: TimerIcon },
  { label: 'Resueltos',   value: stats.resolved,   desc: 'Soluciones completadas',  icon: CheckCircleIcon },
]);

const newTicket = reactive({ title: '', description: '', priority: '1' });
const creatingTicket = ref(false);
const isPriorityDropdownOpen = ref(false);
const formErrors = ref({ title: '', description: '' });

const priorityOptions = {
  '0': { label: 'Baja Importancia',  icon: CheckCircle2Icon,   colorClass: 'text-emerald-500' },
  '1': { label: 'Moderada',          icon: AlertTriangleIcon,  colorClass: 'text-amber-500' },
  '2': { label: 'Urgente / Crítica', icon: AlertOctagonIcon,   colorClass: 'text-rose-500' },
};

const filteredTickets = computed(() => {
  if (!searchQuery.value) return tickets.value;
  return tickets.value.filter(t =>
    t.titulo?.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});

const handleLogout = async () => {
  await authStore.logout();
  router.push('/');
};

const openTicketDetail = async (ticket) => {
  ticketsStore.selectTicket(ticket);
  selectedTicket.value = ticket;
  await ticketsStore.fetchComments(ticket.id);
};

const fetchTickets = async () => {
  notificationStore.info('Cargando tus tickets...', 1500);
  isLoading.value = true;
  try {
    const response = await axios.get(`${API_URL}/tickets/mis-tickets`);
    tickets.value = response.data;
  } catch (err) {
    notificationStore.error('No se pudieron cargar tus tickets.');
  } finally {
    isLoading.value = false;
  }
};

const calculateStats = () => {
  stats.open       = tickets.value.filter(t => t.estado === 'Pendiente' || t.estado === 0).length;
  stats.processing = tickets.value.filter(t => t.estado === 'EnProceso' || t.estado === 1).length;
  stats.resolved   = tickets.value.filter(t => t.estado === 'Resuelto'  || t.estado === 2).length;
};

watch(tickets, calculateStats);

const createTicket = async () => {
  formErrors.value = { title: '', description: '' };
  let hasError = false;
  if (!newTicket.title.trim() || newTicket.title.trim().length < 5) {
    formErrors.value.title = 'El asunto debe tener al menos 5 caracteres';
    hasError = true;
  }
  if (!newTicket.description.trim() || newTicket.description.trim().length < 10) {
    formErrors.value.description = 'La descripción debe tener al menos 10 caracteres';
    hasError = true;
  }
  if (hasError) return;

  creatingTicket.value = true;
  try {
    await axios.post(`${API_URL}/tickets`, {
      titulo: newTicket.title.trim(),
      descripcion: newTicket.description.trim(),
      prioridad: parseInt(newTicket.priority)
    });
    showModal.value = false;
    newTicket.title = '';
    newTicket.description = '';
    notificationStore.success('¡Ticket enviado correctamente!');
    fetchTickets();
  } catch {
    notificationStore.error('Hubo un problema al enviar el ticket.');
  } finally {
    creatingTicket.value = false;
  }
};

onMounted(() => {
  if (!authStore.isAuthenticated) { router.push('/'); return; }
  fetchTickets();

  const closeMenu = (e) => {
    if (!e.target.closest('.profile-menu-container')) showProfileMenu.value = false;
  };
  const handleKeydown = (e) => {
    const tag = e.target.tagName;
    if (tag === 'INPUT' || tag === 'TEXTAREA') return;
    if (e.key === 'n' || e.key === 'N') showModal.value = true;
    if (e.key === 'r' || e.key === 'R') fetchTickets();
    if (e.key === 'Escape') { showModal.value = false; selectedTicket.value = null; }
  };

  window.addEventListener('click', closeMenu);
  window.addEventListener('keydown', handleKeydown);
  onUnmounted(() => {
    window.removeEventListener('click', closeMenu);
    window.removeEventListener('keydown', handleKeydown);
  });
});
</script>