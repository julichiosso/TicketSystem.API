<template>
 <div class="min-h-screen text-slate-900 flex font-sans select-none transition-colors duration-700">
 <Sidebar />

 <!-- Main Content -->
 <main class="flex-1 min-w-0 h-screen overflow-y-auto page-fade-in custom-scrollbar">
      <header class="h-20 flex items-center justify-between px-10 bg-white/[0.01] border-b border-white/[0.02] sticky top-0 z-30">
  <div>
  <h2 class="text-xl font-semibold text-slate-800 tracking-tight">Mis Tickets</h2>
 <p class="text-[10px] text-slate-500 font-bold uppercase tracking-[0.2em]">Soporte y Seguimiento</p>
 </div>

 <div class="flex items-center gap-6">
 <button @click="fetchTickets" class="p-2.5 bg-white/[0.03] hover:bg-white/[0.08] text-slate-500 hover:text-slate-900 rounded-xl transition-all active:rotate-180 duration-700" title="Presiona R para refrescar">
 <RefreshCcwIcon class="w-5 h-5" />
 </button>

 <div class="h-8 w-px bg-white"></div>

 <div class="relative profile-menu-container">
 <button @click.stop="showProfileMenu = !showProfileMenu" class="flex items-center gap-4 group cursor-pointer transition-all">
 <div class="text-right hidden sm:block">
 <div class="text-xs font-black text-slate-900 uppercase tracking-wider group-hover:text-blue-400 transition-colors">{{ authStore.user?.nombre }}</div>
 <div class="text-[9px] text-slate-500 font-bold uppercase tracking-widest">{{ authStore.user?.rol }}</div>
 </div>
 <div :class="`w-10 h-10 rounded-2xl bg-gradient-to-br from-slate-800 to-slate-900 border border-slate-200 flex items-center justify-center ${settingsStore.accentColor} font-black shadow-sm group-hover:border-blue-300 transition-all` ">
 {{ authStore.user?.nombre?.[0] }}
 </div>
 </button>

 <div v-if="showProfileMenu" class="absolute right-0 mt-4 w-56 bg-white border border-slate-200 rounded-3xl shadow-md py-3 z-50 animate-in slide-in-from-top-2 duration-300">
 <router-link to="/profile" class="flex items-center gap-3 px-6 py-3 text-slate-500 hover:text-slate-900 hover:bg-slate-50 transition-all">
 <UserIcon class="w-4 h-4" />
 <span class="text-xs font-bold uppercase tracking-widest">Mi Perfil</span>
 </router-link>
 <router-link to="/settings" class="flex items-center gap-3 px-6 py-3 text-slate-500 hover:text-slate-900 hover:bg-slate-50 transition-all">
 <SettingsIcon class="w-4 h-4" />
 <span class="text-xs font-bold uppercase tracking-widest">Ajustes</span>
 </router-link>
 <div class="h-px bg-white/5 my-2 mx-6"></div>
 <button @click="handleLogout" class="w-full flex items-center gap-3 px-6 py-3 text-rose-500 hover:bg-rose-500/10 transition-all">
 <LogOutIcon class="w-4 h-4" />
 <span class="text-xs font-bold uppercase tracking-widest">Cerrar Sesión</span>
 </button>
 </div>
 </div>
 </div>
 </header>

 <div class="max-w-[1600px] mx-auto transition-all duration-500" :style="{ padding: 'var(--density-p, 2.5rem)' }">
 <!-- Welcome Section -->
 <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
  <div class="bg-white border border-slate-100 shadow-sm rounded-2xl p-6 col-span-1 md:col-span-2 relative overflow-hidden">
  <div class="flex items-center justify-between relative z-10">
  <div>
  <h2 class="text-xl font-bold text-slate-800 mb-1">¡Hola, {{ authStore.user?.nombre }}!</h2>
  <p class="text-slate-500 text-sm">Gestiona y da seguimiento a tus tickets de soporte</p>
  </div>
  <div class="text-5xl opacity-5">🎟️</div>
  </div>
  </div>
  <div class="bg-white border border-slate-100 shadow-sm rounded-2xl p-6 relative overflow-hidden">
  <div class="relative z-10">
  <p class="text-[10px] text-slate-400 font-semibold uppercase tracking-wider mb-2">Tiempo Promedio</p>
  <h3 class="text-xl font-bold text-blue-600 mb-1">~2h</h3>
  <p class="text-xs text-slate-400">Respuesta del equipo</p>
  </div>
  </div>
 </div>
 <!-- Dashboard Stats -->
  <div class="grid grid-cols-1 md:grid-cols-3 gap-8 mb-16">
  <div class="bg-white border border-slate-100 p-8 rounded-2xl relative overflow-hidden group hover:scale-[1.01] transition-all shadow-sm">
  <div class="relative z-10">
  <p class="text-[9px] font-bold uppercase tracking-widest text-slate-400 mb-1">Pendientes</p>
  <h4 class="text-3xl font-bold text-slate-800 mb-2">{{ stats.open }}</h4>
  <p class="text-xs text-slate-400 font-medium">En espera de respuesta</p>
  </div>
  <InboxIcon class="absolute -right-4 -bottom-4 w-24 h-24 text-slate-50 group-hover:scale-110 transition-transform duration-700" />
  </div>

  <div class="bg-white border border-slate-100 p-8 rounded-2xl relative overflow-hidden group hover:scale-[1.01] transition-all shadow-sm">
  <div class="relative z-10">
  <p class="text-[9px] font-bold uppercase tracking-widest text-slate-400 mb-1">En Proceso</p>
  <h4 class="text-3xl font-bold text-slate-800 mb-2">{{ stats.processing }}</h4>
  <p class="text-xs text-slate-400 font-medium">Siendo atendidos</p>
  </div>
  <TimerIcon class="absolute -right-4 -bottom-4 w-24 h-24 text-slate-50 group-hover:scale-110 transition-transform duration-700" />
  </div>

  <div class="bg-white border border-slate-100 p-8 rounded-2xl relative overflow-hidden group hover:scale-[1.01] transition-all shadow-sm">
  <div class="relative z-10">
  <p class="text-[9px] font-bold uppercase tracking-widest text-slate-400 mb-1">Resueltos</p>
  <h4 class="text-3xl font-bold text-slate-800 mb-2">{{ stats.resolved }}</h4>
  <p class="text-xs text-slate-400 font-medium">Soluciones completadas</p>
  </div>
  <CheckCircleIcon class="absolute -right-4 -bottom-4 w-24 h-24 text-emerald-500/5 group-hover:scale-110 transition-transform duration-700" />
  </div>
  </div>

 <!-- Section Title -->
 <div class="flex flex-col sm:flex-row items-center justify-between mb-8 px-2 gap-4">
 <div class="flex items-center gap-6">
  <h3 class="text-base font-bold text-slate-800 flex items-center gap-3">
  <div :class="`w-1 h-5 bg-${settingsStore.themeColor}-600 rounded-full transition-colors` "></div>
  LISTADO DE TICKETS
  </h3>
        <button @click="showModal = true" class="group flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2.5 rounded-lg font-medium transition-colors shadow-sm">
          <PlusIcon class="w-4 h-4 group-hover:rotate-90 transition-transform" />
          <span class="text-sm hidden sm:inline">Nuevo Ticket</span>
        </button>
 </div>

 <!-- search box -->
 <div class="relative w-full max-w-xs">
 <input
 v-model="searchQuery"
 type="text"
 placeholder="Buscar tickets..."
 class="w-full bg-transparent border border-slate-200 px-4 py-2 rounded-xl text-sm placeholder-slate-400 focus:border-blue-500 outline-none transition"
 />
 <SearchIcon class="w-4 h-4 absolute right-3 top-1/2 transform -translate-y-1/2 text-slate-500" />
 </div>
 
 <div class="text-[10px] text-slate-500 font-bold uppercase tracking-widest bg-white/5 px-4 py-2 rounded-full border border-slate-200">
 Total: {{ filteredTickets.length }} tickets
 </div>
 </div>

 <!-- loading skeletons -->
 <div v-if="isLoading" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
 <TicketCardSkeleton v-for="n in 8" :key="n" />
 </div>

 <!-- tickets list -->
 <div v-else-if="filteredTickets.length > 0" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
 <TicketCard 
 v-for="ticket in filteredTickets" 
 :key="ticket.id" 
 :ticket="ticket" 
 @click="openTicketDetail(ticket)" 
 class="cursor-pointer" 
 />
 </div>

 <div v-else class="flex flex-col items-center justify-center py-32 border-2 border-dashed border-slate-200 rounded-[3rem] bg-white group transition-all duration-700">
 <div class="w-24 h-24 bg-white rounded-full flex items-center justify-center mb-8 border border-slate-200 group-hover:scale-110 group-hover:border-blue-300 transition-all duration-700">
 <InboxIcon class="w-10 h-10 text-slate-700 group-hover:text-blue-500 transition-colors" />
 </div>
 <h3 class="text-xl font-black text-slate-900 mb-2 uppercase italic tracking-tight">
 {{ tickets.length === 0 ? 'Tu buzón está vacío' : 'Sin resultados' }}
 </h3>
 <p class="text-slate-500 text-sm max-w-sm text-center mb-10 font-medium leading-relaxed">
 <template v-if="tickets.length === 0">
 Parece que no tenés tickets activos actualmente. Si necesitás soporte, creá uno nuevo.
 </template>
 <template v-else>
 No se encontraron tickets que coincidan con "{{ searchQuery }}".
 </template>
 </p>
        <button v-if="tickets.length === 0" @click="showModal = true" class="group flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white font-medium px-6 py-3 rounded-lg shadow-sm transition-colors">
          <PlusIcon class="w-5 h-5 group-hover:rotate-90 transition-transform duration-300" />
          Crear mi primer ticket
        </button>
 </div>
 </div>
 </main>

 <!-- Create Ticket Modal -->
 <div v-if="showModal" class="fixed inset-0 z-[100] flex items-center justify-center p-6 bg-slate-50/70 transition-all duration-500">
 <div class="bg-white border border-slate-200 rounded-[3rem] w-full max-w-xl shadow-md relative">
 <div class="p-10 relative z-10">
 <div class="flex justify-between items-start mb-10">
 <div>
              <h3 class="text-2xl font-bold text-slate-900">Nuevo Ticket</h3>
              <p class="text-sm text-slate-500 mt-1">Ingresa los detalles de tu problema</p>
 </div>
 <button @click="showModal = false" class="text-slate-400 hover:text-slate-600 transition-colors p-2 hover:bg-slate-100 rounded-full">
            <XIcon class="w-5 h-5" />
          </button>
 </div>

        <!-- Scrollable Content with visible dropdowns -->
        <div class="px-8 pb-8 flex-1">
          <!-- Form Header -->
          <div class="mb-8">
            <h3 class="text-2xl font-black text-slate-900 tracking-tight">Nuevo Ticket</h3>
          </div>

 <form @submit.prevent="createTicket" class="space-y-8">
 <div class="space-y-4">
 <div class="relative group">
 <label class="absolute -top-2.5 left-4 bg-white px-2 text-[9px] font-black text-slate-500 uppercase tracking-widest z-10 transition-colors group-focus-within:text-blue-500">Asunto del Problema</label>
 <input v-model="newTicket.title" @input="formErrors.title = ''" maxlength="100" class="w-full bg-transparent border-2 border-slate-200 text-slate-900 px-6 py-4 rounded-2xl focus:border-blue-500 outline-none transition-all font-medium placeholder:font-normal placeholder:text-slate-400" placeholder="Ej: Error al procesar pago" />
 <p v-if="formErrors.title" class="text-rose-500 text-xs font-medium mt-1.5 flex items-center gap-1">
   <AlertCircleIcon class="w-3.5 h-3.5 flex-shrink-0" /> {{ formErrors.title }}
 </p>
 <div class="text-[8px] text-slate-500 mt-1">{{ newTicket.title.length }}/100</div>
 </div>

 <div class="relative group">
 <label class="absolute -top-2.5 left-4 bg-white px-2 text-[9px] font-black text-slate-500 uppercase tracking-widest z-10 transition-colors group-focus-within:text-blue-500">Descripción Detallada</label>
 <textarea v-model="newTicket.description" @input="formErrors.description = ''" rows="4" maxlength="1000" class="w-full bg-transparent border-2 border-slate-200 text-slate-900 px-6 py-4 rounded-2xl focus:border-blue-500 outline-none transition-all font-medium placeholder:font-normal placeholder:text-slate-400" placeholder="¿Qué está fallando exactamente?"></textarea>
 <p v-if="formErrors.description" class="text-rose-500 text-xs font-medium mt-1.5 flex items-center gap-1">
   <AlertCircleIcon class="w-3.5 h-3.5 flex-shrink-0" /> {{ formErrors.description }}
 </p>
 <div class="text-[8px] text-slate-500 mt-1">{{ newTicket.description.length }}/1000</div>
 </div>

            <div class="relative group w-1/2" v-click-outside="() => isPriorityDropdownOpen = false">
              <label class="absolute -top-2.5 left-4 bg-white px-2 text-[9px] font-black text-slate-500 uppercase tracking-widest z-10 transition-colors group-focus-within:text-blue-500">Nivel de Prioridad</label>
              
              <div 
                @click="isPriorityDropdownOpen = !isPriorityDropdownOpen"
                class="w-full bg-transparent border-2 border-slate-200 text-slate-900 px-6 py-4 rounded-2xl cursor-pointer hover:border-blue-400 focus:border-blue-500 outline-none transition-all font-bold flex justify-between items-center"
              >
                <div class="flex items-center gap-3">
                  <component :is="priorityOptions[newTicket.priority].icon" :class="priorityOptions[newTicket.priority].colorClass" class="w-5 h-5" />
                  <span>{{ priorityOptions[newTicket.priority].label }}</span>
                </div>
                <ChevronDownIcon class="w-5 h-5 text-slate-400 transition-transform" :class="isPriorityDropdownOpen ? 'rotate-180' : ''" />
              </div>

              <!-- Dropdown Menu -->
              <div 
                v-if="isPriorityDropdownOpen" 
                class="absolute bottom-full left-0 mb-2 w-full bg-white border border-slate-200 rounded-2xl shadow-[0_10px_40px_-10px_rgba(0,0,0,0.15)] z-50 overflow-hidden animate-in fade-in slide-in-from-bottom-2 duration-200"
              >
                <button 
                  v-for="(option, key) in priorityOptions" 
                  :key="key"
                  type="button"
                  @click="newTicket.priority = key; isPriorityDropdownOpen = false"
                  class="w-full px-6 py-4 flex items-center gap-3 hover:bg-slate-50 transition-colors text-left"
                  :class="newTicket.priority === key ? 'bg-blue-50' : ''"
                >
                  <component :is="option.icon" :class="option.colorClass" class="w-5 h-5" />
                  <span class="font-bold text-slate-700" :class="newTicket.priority === key ? 'text-blue-700' : ''">{{ option.label }}</span>
                </button>
              </div>
            </div>
 </div>

            <div class="flex gap-4 pt-6 mt-4 border-t border-slate-100 relative z-0">
              <button @click="showModal = false" type="button" class="px-6 py-2.5 bg-white border border-slate-300 hover:bg-slate-50 text-slate-700 font-medium rounded-lg transition-colors">
                Cancelar
              </button>
              <button type="submit" :disabled="creatingTicket" class="flex-1 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg shadow-sm transition-colors flex items-center justify-center gap-2 disabled:opacity-50">
                <span v-if="!creatingTicket">Enviar Solicitud</span>
                <span v-else>Enviando...</span>
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>

 <!-- Ticket Detail Modal -->
 <TicketDetailModal
 v-if="selectedTicket"
 :ticket="selectedTicket"
 @close="selectedTicket = null; ticketsStore.clearSelection()"
 />
 </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, reactive, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import {
 InboxIcon, TimerIcon, CheckCircleIcon, RefreshCcwIcon, XIcon, ArrowRightIcon,
 PlusIcon, UserIcon, SettingsIcon, SearchIcon, PlusCircleIcon, ChevronDownIcon,
 CheckCircle2Icon, AlertTriangleIcon, AlertOctagonIcon, AlertCircleIcon
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

const newTicket = reactive({
 title: '',
 description: '',
 priority: '1'
});
const creatingTicket = ref(false);
const isPriorityDropdownOpen = ref(false);
const formErrors = ref({ title: '', description: '' });

const priorityOptions = {
  '0': { label: 'Baja Importancia', icon: CheckCircle2Icon, colorClass: 'text-emerald-500' },
  '1': { label: 'Moderada', icon: AlertTriangleIcon, colorClass: 'text-amber-500' },
  '2': { label: 'Urgente / Crítica', icon: AlertOctagonIcon, colorClass: 'text-rose-500' }
};

// derived list after search / filters
const filteredTickets = computed(() => {
 if (!searchQuery.value) return tickets.value;
 return tickets.value.filter(t =>
 t.titulo?.toLowerCase().includes(searchQuery.value.toLowerCase())
 );
});

const handleLogout = () => {
 authStore.logout();
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
 calculateStats();
 } catch (err) {
 console.error('Error fetching tickets:', err);
 notificationStore.error('No se pudieron cargar tus tickets.');
 } finally {
 isLoading.value = false;
 }
};

const calculateStats = () => {
 stats.open = tickets.value.filter(t => t.estado === 'Pendiente' || t.estado === 0).length;
 stats.processing = tickets.value.filter(t => t.estado === 'EnProceso' || t.estado === 1).length;
 stats.resolved = tickets.value.filter(t => t.estado === 'Resuelto' || t.estado === 2).length;
};

// recompute whenever tickets array changes (e.g. after fetch or real‑time update)
watch(tickets, calculateStats);

const createTicket = async () => {
 formErrors.value = { title: '', description: '' };
 let hasError = false;

 // Validate inputs
 if (!newTicket.title.trim()) {
   formErrors.value.title = 'El asunto es requerido';
   hasError = true;
 } else if (newTicket.title.trim().length < 5) {
   formErrors.value.title = 'El asunto debe tener al menos 5 caracteres';
   hasError = true;
 }

 if (!newTicket.description.trim()) {
   formErrors.value.description = 'La descripción es requerida';
   hasError = true;
 } else if (newTicket.description.trim().length < 10) {
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
 } catch (err) {
 console.error('Error creating ticket:', err);
 notificationStore.error('Hubo un problema al enviar el ticket.');
 } finally {
 creatingTicket.value = false;
 }
};

onMounted(() => {
 if (!authStore.isAuthenticated) {
 router.push('/');
 return;
 }

 fetchTickets();

 const closeMenu = (e) => {
 if (!e.target.closest('.profile-menu-container')) {
 showProfileMenu.value = false;
 }
 };

 const handleKeydown = (e) => {
 // ignore typing inside inputs/textareas
 const tag = e.target.tagName;
 if (tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT') return;
 if (e.key === 'n' || e.key === 'N') {
 showModal.value = true;
 }
 if (e.key === 'r' || e.key === 'R') {
 fetchTickets();
 }
 if (e.key === '/') {
 // quick search focus
 e.preventDefault();
 const searchEl = document.querySelector('input[placeholder="Buscar tickets..."]');
 searchEl?.focus();
 }
 if (e.key === 'Escape') {
 showModal.value = false;
 selectedTicket.value = null;
 ticketsStore.clearSelection();
 }
 };

 const offlineHandler = () => {
 notificationStore.warning('Estás sin conexión. Algunos datos pueden estar desactualizados.');
 };

 const onlineHandler = () => {
 notificationStore.success('Conexión reestablecida.');
 fetchTickets();
 };

 window.addEventListener('click', closeMenu);
 window.addEventListener('keydown', handleKeydown);
 window.addEventListener('offline', offlineHandler);
 window.addEventListener('online', onlineHandler);

 // cleanup when component is unmounted
 onUnmounted(() => {
 window.removeEventListener('click', closeMenu);
 window.removeEventListener('keydown', handleKeydown);
 window.removeEventListener('offline', offlineHandler);
 window.removeEventListener('online', onlineHandler);
 });
});
</script>

<style>
.custom-scrollbar::-webkit-scrollbar {
 width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
 background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
 background: rgba(255, 255, 255, 0.05);
 border-radius: 10px;
}
</style>
