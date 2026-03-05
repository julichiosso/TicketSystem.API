<template>
  <div class="min-h-screen flex font-sans select-none transition-colors duration-300"
    :class="settingsStore.isDark ? 'bg-slate-950 text-white' : 'bg-slate-50 text-slate-900'">

    <Sidebar />

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
          <button @click="recargar"
            class="p-2.5 rounded-xl transition-all active:rotate-180 duration-700"
            :class="settingsStore.isDark ? 'hover:bg-slate-800 text-slate-400' : 'hover:bg-slate-100 text-slate-500'">
            <RefreshCcwIcon class="w-5 h-5" />
          </button>
          <div class="h-8 w-px" :class="settingsStore.isDark ? 'bg-slate-700' : 'bg-slate-200'"></div>
          <div class="relative profile-menu-container">
            <button @click.stop="showProfileMenu = !showProfileMenu"
              class="flex items-center gap-3 group cursor-pointer">
              <div class="text-right hidden sm:block">
                <div class="text-xs font-black uppercase tracking-wider group-hover:text-blue-400 transition-colors">
                  {{ authStore.user?.nombre }}
                </div>
                <div class="text-[9px] font-bold uppercase tracking-widest"
                  :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
                  {{ authStore.user?.rol }}
                </div>
              </div>
              <div :class="`w-9 h-9 rounded-xl bg-gradient-to-br ${settingsStore.themeClasses} flex items-center justify-center font-black text-white text-sm shadow-sm`">
                {{ authStore.user?.nombre?.[0] }}
              </div>
            </button>
            <div v-if="showProfileMenu"
              class="absolute right-0 mt-3 w-52 rounded-2xl shadow-lg py-2 z-50 border"
              :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200'">
              <router-link to="/profile"
                class="flex items-center gap-3 px-5 py-2.5 transition-all text-sm font-medium"
                :class="settingsStore.isDark ? 'text-slate-400 hover:text-white hover:bg-slate-800' : 'text-slate-500 hover:text-slate-900 hover:bg-slate-50'">
                <UserIcon class="w-4 h-4" /> Mi Perfil
              </router-link>
              <router-link to="/settings"
                class="flex items-center gap-3 px-5 py-2.5 transition-all text-sm font-medium"
                :class="settingsStore.isDark ? 'text-slate-400 hover:text-white hover:bg-slate-800' : 'text-slate-500 hover:text-slate-900 hover:bg-slate-50'">
                <SettingsIcon class="w-4 h-4" /> Ajustes
              </router-link>
              <div class="h-px my-1 mx-4" :class="settingsStore.isDark ? 'bg-slate-800' : 'bg-slate-100'"></div>
              <button @click="handleLogout"
                class="w-full flex items-center gap-3 px-5 py-2.5 text-rose-500 hover:bg-rose-500/10 transition-all text-sm font-medium">
                <LogOutIcon class="w-4 h-4" /> Cerrar Sesión
              </button>
            </div>
          </div>
        </div>
      </header>

      <div class="max-w-[1600px] mx-auto p-10">

        <!-- Welcome + Stats -->
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
          <div class="md:col-span-3 rounded-2xl p-6 border transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-100 shadow-sm'">
            <div class="flex items-center justify-between">
              <div>
                <h2 class="text-xl font-bold mb-1">¡Hola, {{ authStore.user?.nombre }}!</h2>
                <p class="text-sm" :class="settingsStore.isDark ? 'text-slate-400' : 'text-slate-500'">
                  Gestiona y da seguimiento a tus tickets de soporte
                </p>
              </div>
              <div class="text-5xl opacity-10">🎟️</div>
            </div>
          </div>
          <div class="rounded-2xl p-6 border transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-100 shadow-sm'">
            <p class="text-[10px] font-semibold uppercase tracking-wider mb-1"
              :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">Tiempo Promedio</p>
            <h3 class="text-2xl font-bold text-blue-500">~2h</h3>
            <p class="text-xs mt-1" :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">Respuesta</p>
          </div>
        </div>

        <!-- Stats -->
        <div class="grid grid-cols-3 gap-4 mb-8">
          <div v-for="stat in statCards" :key="stat.label"
            class="rounded-2xl p-6 border transition-all hover:scale-[1.01]"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-100 shadow-sm'">
            <p class="text-[9px] font-bold uppercase tracking-widest mb-1"
              :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">{{ stat.label }}</p>
            <h4 class="text-3xl font-bold mb-1">{{ stat.value }}</h4>
            <p class="text-xs" :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">{{ stat.desc }}</p>
          </div>
        </div>

        <!-- Toolbar -->
        <div class="flex flex-col sm:flex-row items-center justify-between mb-6 gap-4">
          <div class="flex items-center gap-4">
            <h3 class="text-sm font-black flex items-center gap-3 uppercase tracking-widest">
              <div :class="`w-1 h-5 bg-gradient-to-b ${settingsStore.themeClasses} rounded-full`"></div>
              Mis Tickets
            </h3>
            <button @click="showModal = true"
              class="group flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium text-sm transition-colors shadow-sm">
              <PlusIcon class="w-4 h-4 transition-transform group-hover:rotate-90 duration-300" />
              Nuevo Ticket
            </button>
          </div>
          <div class="flex items-center gap-3">
            <div class="relative">
              <input v-model="searchQuery" type="text" placeholder="Buscar..."
                class="border pl-9 pr-4 py-2 rounded-xl text-sm outline-none transition w-56"
                :class="settingsStore.isDark
                  ? 'bg-slate-900 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
                  : 'bg-white border-slate-200 placeholder-slate-400 focus:border-blue-500'" />
              <SearchIcon class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2"
                :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'" />
            </div>
            <div class="flex gap-1">
              <button v-for="f in filtrosRapidos" :key="f.key"
                @click="toggleFiltroRapido(f.key)"
                class="px-3 py-1.5 rounded-lg text-[10px] font-black uppercase tracking-widest border transition-all"
                :class="filtroActivo === f.key
                  ? 'bg-blue-600 border-blue-600 text-white'
                  : settingsStore.isDark
                    ? 'bg-slate-900 border-slate-700 text-slate-500 hover:border-slate-600'
                    : 'bg-white border-slate-200 text-slate-400 hover:border-slate-300'">
                {{ f.label }}
              </button>
            </div>
            <div class="text-[10px] font-bold uppercase tracking-widest px-3 py-1.5 rounded-lg border"
              :class="settingsStore.isDark ? 'bg-slate-900 border-slate-700 text-slate-500' : 'bg-white border-slate-200 text-slate-400'">
              {{ filteredAll.length }} tickets
            </div>
          </div>
        </div>

        <!-- Loading -->
        <div v-if="isLoading"
          class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCardSkeleton v-for="n in 8" :key="n" />
        </div>

        <!-- Tickets -->
        <div v-else-if="tickets.length > 0"
          class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCard v-for="ticket in tickets" :key="ticket.id"
            :ticket="ticket" @click="openTicketDetail(ticket)" class="cursor-pointer" />
        </div>

        <!-- Empty -->
        <div v-else
          class="flex flex-col items-center justify-center py-32 border-2 border-dashed rounded-[3rem] transition-colors"
          :class="settingsStore.isDark ? 'border-slate-800 bg-slate-900/50' : 'border-slate-200 bg-white'">
          <InboxIcon class="w-10 h-10 mb-4"
            :class="settingsStore.isDark ? 'text-slate-700' : 'text-slate-300'" />
          <h3 class="text-xl font-black mb-2 uppercase italic">
            {{ allTickets.length === 0 ? 'Tu buzón está vacío' : 'Sin resultados' }}
          </h3>
          <p class="text-sm max-w-sm text-center font-medium"
            :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
            {{ allTickets.length === 0
              ? 'No tenés tickets activos. Creá uno nuevo si necesitás soporte.'
              : `Sin resultados para "${searchQuery}".` }}
          </p>
          <button v-if="allTickets.length === 0" @click="showModal = true"
            class="mt-6 flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white font-medium px-6 py-3 rounded-lg transition-colors">
            <PlusIcon class="w-5 h-5" /> Crear mi primer ticket
          </button>
        </div>

        <!-- Paginación -->
        <div v-if="totalPages > 1"
          class="flex items-center justify-between mt-8 pt-6 border-t"
          :class="settingsStore.isDark ? 'border-slate-800' : 'border-slate-100'">

          <p class="text-xs font-medium"
            :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
            Mostrando
            <span class="font-black" :class="settingsStore.isDark ? 'text-white' : 'text-slate-900'">
              {{ (currentPage - 1) * PAGE_SIZE + 1 }}–{{ Math.min(currentPage * PAGE_SIZE, filteredAll.length) }}
            </span>
            de
            <span class="font-black" :class="settingsStore.isDark ? 'text-white' : 'text-slate-900'">
              {{ filteredAll.length }}
            </span>
            tickets
          </p>

          <div class="flex items-center gap-1">
            <button @click="goToPage(currentPage - 1)" :disabled="currentPage === 1"
              class="p-2 rounded-lg border transition-all disabled:opacity-30 disabled:cursor-not-allowed"
              :class="settingsStore.isDark ? 'bg-slate-800 border-slate-700 text-slate-400 hover:border-slate-600' : 'bg-white border-slate-200 text-slate-500 hover:border-slate-300'">
              <ChevronLeftIcon class="w-4 h-4" />
            </button>
            <template v-for="p in visiblePages" :key="p">
              <span v-if="p === '...'" class="px-2 text-xs"
                :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">···</span>
              <button v-else @click="goToPage(p)"
                class="w-8 h-8 rounded-lg border text-xs font-black transition-all"
                :class="p === currentPage
                  ? 'bg-blue-600 border-blue-600 text-white shadow-sm'
                  : settingsStore.isDark
                    ? 'bg-slate-800 border-slate-700 text-slate-400 hover:border-slate-600'
                    : 'bg-white border-slate-200 text-slate-600 hover:border-slate-300'">
                {{ p }}
              </button>
            </template>
            <button @click="goToPage(currentPage + 1)" :disabled="currentPage === totalPages"
              class="p-2 rounded-lg border transition-all disabled:opacity-30 disabled:cursor-not-allowed"
              :class="settingsStore.isDark ? 'bg-slate-800 border-slate-700 text-slate-400 hover:border-slate-600' : 'bg-white border-slate-200 text-slate-500 hover:border-slate-300'">
              <ChevronRightIcon class="w-4 h-4" />
            </button>
          </div>

          <p class="text-[10px] font-bold uppercase tracking-widest"
            :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
            Página {{ currentPage }} de {{ totalPages }}
          </p>
        </div>

      </div>
    </main>

    <!-- Modal Nuevo Ticket -->
    <div v-if="showModal"
      class="fixed inset-0 z-[100] flex items-center justify-center p-6 backdrop-blur-sm"
      :class="settingsStore.isDark ? 'bg-slate-950/70' : 'bg-slate-900/30'">
      <div class="rounded-[2.5rem] w-full max-w-xl shadow-xl border"
        :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200'">
        <div class="p-10">
          <div class="flex justify-between items-start mb-8">
            <div>
              <h3 class="text-2xl font-bold">Nuevo Ticket</h3>
              <p class="text-sm mt-1" :class="settingsStore.isDark ? 'text-slate-400' : 'text-slate-500'">
                Describí tu problema con detalle
              </p>
            </div>
            <button @click="showModal = false"
              class="p-2 rounded-full transition-all"
              :class="settingsStore.isDark ? 'hover:bg-slate-800 text-slate-400' : 'hover:bg-slate-100 text-slate-400'">
              <XIcon class="w-5 h-5" />
            </button>
          </div>
          <form @submit.prevent="createTicket" class="space-y-5">
            <div class="relative">
              <label class="absolute -top-2.5 left-4 px-2 text-[9px] font-black uppercase tracking-widest z-10"
                :class="settingsStore.isDark ? 'bg-slate-900 text-slate-500' : 'bg-white text-slate-400'">
                Asunto
              </label>
              <input v-model="newTicket.title" maxlength="100"
                class="w-full border-2 px-5 py-3.5 rounded-2xl outline-none transition font-medium text-sm"
                :class="settingsStore.isDark
                  ? 'bg-slate-800 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
                  : 'bg-white border-slate-200 text-slate-900 focus:border-blue-500'"
                placeholder="Ej: Error al procesar pago" />
              <p v-if="formErrors.title" class="text-rose-500 text-xs mt-1 flex items-center gap-1">
                <AlertCircleIcon class="w-3.5 h-3.5" /> {{ formErrors.title }}
              </p>
            </div>
            <div class="relative">
              <label class="absolute -top-2.5 left-4 px-2 text-[9px] font-black uppercase tracking-widest z-10"
                :class="settingsStore.isDark ? 'bg-slate-900 text-slate-500' : 'bg-white text-slate-400'">
                Descripción
              </label>
              <textarea v-model="newTicket.description" rows="4" maxlength="1000"
                class="w-full border-2 px-5 py-3.5 rounded-2xl outline-none transition font-medium text-sm resize-none"
                :class="settingsStore.isDark
                  ? 'bg-slate-800 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
                  : 'bg-white border-slate-200 text-slate-900 focus:border-blue-500'"
                placeholder="¿Qué está fallando exactamente?"></textarea>
              <p v-if="formErrors.description" class="text-rose-500 text-xs mt-1 flex items-center gap-1">
                <AlertCircleIcon class="w-3.5 h-3.5" /> {{ formErrors.description }}
              </p>
            </div>
            <div class="flex gap-3">
              <button v-for="(opt, key) in priorityOptions" :key="key" type="button"
                @click="newTicket.priority = key"
                class="flex-1 py-3 rounded-xl border-2 flex flex-col items-center gap-1 transition-all text-xs font-black uppercase tracking-widest"
                :class="newTicket.priority === key
                  ? 'border-blue-500 bg-blue-500/10 text-blue-500'
                  : settingsStore.isDark
                    ? 'border-slate-700 text-slate-500 hover:border-slate-600'
                    : 'border-slate-200 text-slate-400 hover:border-slate-300'">
                <component :is="opt.icon" class="w-4 h-4" :class="opt.colorClass" />
                {{ opt.label }}
              </button>
            </div>
            <div class="flex gap-3 pt-2 border-t"
              :class="settingsStore.isDark ? 'border-slate-800' : 'border-slate-100'">
              <button @click="showModal = false" type="button"
                class="px-5 py-2.5 border font-medium rounded-lg text-sm transition-colors"
                :class="settingsStore.isDark
                  ? 'bg-slate-800 border-slate-700 text-slate-300 hover:bg-slate-700'
                  : 'bg-white border-slate-300 text-slate-700 hover:bg-slate-50'">
                Cancelar
              </button>
              <button type="submit" :disabled="creatingTicket"
                class="flex-1 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg shadow-sm transition-colors disabled:opacity-50">
                {{ creatingTicket ? 'Enviando...' : 'Enviar Solicitud' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <TicketDetailModal v-if="selectedTicket" :ticket="selectedTicket"
      @close="selectedTicket = null; ticketsStore.clearSelection()" />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import {
  InboxIcon, RefreshCcwIcon, XIcon, PlusIcon,
  UserIcon, SettingsIcon, SearchIcon, LogOutIcon,
  CheckCircle2Icon, AlertTriangleIcon, AlertOctagonIcon, AlertCircleIcon,
  ChevronLeftIcon, ChevronRightIcon
} from 'lucide-vue-next';
import { useAuthStore, API_URL } from '../store/auth';
import { useNotificationStore } from '../store/notifications';
import { useSettingsStore } from '../store/settings';
import { useTicketsStore } from '../store/tickets';
import TicketCard from '../components/TicketCard.vue';
import TicketCardSkeleton from '../components/TicketCardSkeleton.vue';
import TicketDetailModal from '../components/TicketDetailModal.vue';
import Sidebar from '../components/Sidebar.vue';

const authStore         = useAuthStore();
const notificationStore = useNotificationStore();
const settingsStore     = useSettingsStore();
const ticketsStore      = useTicketsStore();
const router            = useRouter();

const allTickets      = ref([]);
const isLoading       = ref(false);
const searchQuery     = ref('');
const showModal       = ref(false);
const showProfileMenu = ref(false);
const selectedTicket  = ref(null);
const filtroActivo    = ref(null);
const currentPage     = ref(1);
const PAGE_SIZE       = 12;

const stats = reactive({ open: 0, processing: 0, resolved: 0 });

const statCards = computed(() => [
  { label: 'Pendientes', value: stats.open,       desc: 'En espera' },
  { label: 'En Proceso', value: stats.processing, desc: 'Siendo atendidos' },
  { label: 'Resueltos',  value: stats.resolved,   desc: 'Completados' },
]);

const filtrosRapidos = [
  { key: null,        label: 'Todos' },
  { key: 'Pendiente', label: 'Pendientes' },
  { key: 'EnProceso', label: 'En Proceso' },
  { key: 'Resuelto',  label: 'Resueltos' },
];

const toggleFiltroRapido = (key) => {
  filtroActivo.value = filtroActivo.value === key ? null : key;
  currentPage.value  = 1;
};

// Tickets filtrados por búsqueda y filtro rápido
const filteredAll = computed(() => {
  let result = allTickets.value;
  if (filtroActivo.value)
    result = result.filter(t => t.estado === filtroActivo.value);
  if (searchQuery.value)
    result = result.filter(t =>
      t.titulo?.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  return result;
});

// Tickets de la página actual
const tickets = computed(() => {
  const start = (currentPage.value - 1) * PAGE_SIZE;
  return filteredAll.value.slice(start, start + PAGE_SIZE);
});

const totalPages = computed(() => Math.ceil(filteredAll.value.length / PAGE_SIZE) || 1);

const goToPage = (p) => {
  if (p < 1 || p > totalPages.value) return;
  currentPage.value = p;
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

const visiblePages = computed(() => {
  const total   = totalPages.value;
  const current = currentPage.value;
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
  const pages = [1];
  if (current > 3) pages.push('...');
  for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++)
    pages.push(i);
  if (current < total - 2) pages.push('...');
  pages.push(total);
  return pages;
});

// Formulario
const newTicket      = reactive({ title: '', description: '', priority: '1' });
const creatingTicket = ref(false);
const formErrors     = ref({ title: '', description: '' });

const priorityOptions = {
  '0': { label: 'Baja',    icon: CheckCircle2Icon,  colorClass: 'text-emerald-500' },
  '1': { label: 'Media',   icon: AlertTriangleIcon, colorClass: 'text-amber-500' },
  '2': { label: 'Urgente', icon: AlertOctagonIcon,  colorClass: 'text-rose-500' },
};

const calcStats = (all) => {
  stats.open       = all.filter(t => t.estado === 'Pendiente' || t.estado === 0).length;
  stats.processing = all.filter(t => t.estado === 'EnProceso' || t.estado === 1).length;
  stats.resolved   = all.filter(t => t.estado === 'Resuelto'  || t.estado === 2).length;
};

const fetchAll = async () => {
  isLoading.value = true;
  try {
    const response   = await axios.get(`${API_URL}/tickets/mis-tickets`);
    allTickets.value = Array.isArray(response.data) ? response.data : [];
    calcStats(allTickets.value);
  } catch {
    notificationStore.error('No se pudieron cargar los tickets.');
  } finally {
    isLoading.value = false;
  }
};

const recargar = async () => {
  currentPage.value = 1;
  await fetchAll();
  notificationStore.success('Tickets actualizados.');
};

const openTicketDetail = async (ticket) => {
  ticketsStore.selectTicket(ticket);
  selectedTicket.value = ticket;
  await ticketsStore.fetchComments(ticket.id);
};

const handleLogout = async () => {
  await authStore.logout();
  router.push('/');
};

const createTicket = async () => {
  formErrors.value = { title: '', description: '' };
  let hasError = false;
  if (!newTicket.title.trim() || newTicket.title.trim().length < 5) {
    formErrors.value.title = 'El asunto debe tener al menos 5 caracteres.'; hasError = true;
  }
  if (!newTicket.description.trim() || newTicket.description.trim().length < 10) {
    formErrors.value.description = 'La descripción debe tener al menos 10 caracteres.'; hasError = true;
  }
  if (hasError) return;

  creatingTicket.value = true;
  try {
    await axios.post(`${API_URL}/tickets`, {
      titulo:      newTicket.title.trim(),
      descripcion: newTicket.description.trim(),
      prioridad:   parseInt(newTicket.priority)
    });
    showModal.value       = false;
    newTicket.title       = '';
    newTicket.description = '';
    notificationStore.success('¡Ticket enviado!');
    await recargar();
  } catch {
    notificationStore.error('Error al enviar el ticket.');
  } finally {
    creatingTicket.value = false;
  }
};

onMounted(async () => {
  if (!authStore.isAuthenticated) { router.push('/'); return; }
  await fetchAll();

  const closeMenu = (e) => {
    if (!e.target.closest('.profile-menu-container')) showProfileMenu.value = false;
  };
  const onKey = (e) => {
    if (['INPUT', 'TEXTAREA'].includes(e.target.tagName)) return;
    if (e.key === 'n' || e.key === 'N') showModal.value = true;
    if (e.key === 'r' || e.key === 'R') recargar();
    if (e.key === 'Escape') { showModal.value = false; selectedTicket.value = null; }
    if (e.key === 'ArrowRight') goToPage(currentPage.value + 1);
    if (e.key === 'ArrowLeft')  goToPage(currentPage.value - 1);
  };
  window.addEventListener('click', closeMenu);
  window.addEventListener('keydown', onKey);
  onUnmounted(() => {
    window.removeEventListener('click', closeMenu);
    window.removeEventListener('keydown', onKey);
  });
});
</script>