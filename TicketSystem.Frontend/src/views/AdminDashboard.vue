<template>
  <div class="min-h-screen flex transition-colors duration-300"
    :class="'bg-slate-50 dark:bg-slate-950 text-slate-900 dark:text-white'">
    <Sidebar />
    <main class="flex-1 p-8 space-y-6 overflow-y-auto h-screen custom-scrollbar">
      <header class="flex flex-col gap-6 md:flex-row md:items-center justify-between">
        <div>
          <h2 class="text-xl font-semibold tracking-tight">Suite de Control</h2>
          <p class="text-[10px] font-medium uppercase tracking-[0.1em]"
            :class="'text-slate-400 dark:text-slate-500'">
            Administración del Sistema
          </p>
        </div>
        <div class="flex flex-wrap items-center gap-4">
          <div class="p-1 rounded-xl border flex flex-wrap gap-1"
            :class="'bg-slate-100 border-slate-200 dark:bg-slate-900 dark:border-slate-800'">
            <button v-for="tab in tabs" :key="tab.key" @click="activeTab = tab.key"
              class="px-5 py-2 rounded-lg text-[10px] font-bold uppercase tracking-widest transition-all duration-200 whitespace-nowrap"
              :class="activeTab === tab.key
                ? settingsStore.isDark ? 'bg-slate-800 text-blue-400 shadow-sm' : 'bg-white text-blue-600 shadow-sm'
                : settingsStore.isDark ? 'text-slate-500 hover:text-slate-300' : 'text-slate-400 hover:text-slate-600'">
              {{ tab.label }}
            </button>
          </div>
          <div class="flex items-center gap-2">
            <div v-if="activeTab === 'dashboard' || activeTab === 'usuarios'" class="relative w-48 xl:w-64">
              <input v-if="activeTab === 'usuarios'" v-model="usersSearch" placeholder="Buscar usuarios..."
                class="w-full border px-4 py-2 rounded-xl text-sm outline-none transition-all"
                :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 placeholder-slate-400 focus:border-blue-400 dark:text-white dark:placeholder-slate-600 dark:focus:border-blue-500'" />
              <input v-else v-model="ticketsSearch" placeholder="Buscar tickets..."
                class="w-full border px-4 py-2 rounded-xl text-sm outline-none transition-all"
                :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-700 placeholder-slate-400 focus:border-blue-400 dark:text-white dark:placeholder-slate-600 dark:focus:border-blue-500'" />
              <SearchIcon class="w-4 h-4 absolute right-3 top-1/2 -translate-y-1/2"
                :class="'text-slate-400 dark:text-slate-600'" />
            </div>
            <button v-if="activeTab === 'dashboard'" @click="fetchTickets"
              class="p-2.5 rounded-xl transition-all active:rotate-180 duration-700"
              :class="'hover:bg-slate-200 text-slate-500 dark:hover:bg-slate-800 dark:text-slate-400'">
              <RefreshCcwIcon class="w-5 h-5" />
            </button>
          </div>
        </div>
      </header>

      <section v-if="activeTab === 'dashboard'" class="space-y-4">
        <div class="grid md:grid-cols-5 gap-3">
          <div v-for="metric in metrics" :key="metric.label"
            class="rounded-xl p-4 border transition-all hover:scale-[1.02]"
            :class="'bg-white dark:bg-slate-900 border-slate-100 dark:border-slate-800 shadow-sm'">
            <div class="text-[10px] font-semibold uppercase tracking-wider mb-1"
              :class="'text-slate-400 dark:text-slate-500'">{{ metric.label }}</div>
            <div class="text-xl font-bold">{{ metric.value }}</div>
          </div>
        </div>
        <TicketFilters />
        <div v-if="isLoadingTickets" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCardSkeleton v-for="n in 8" :key="n" />
        </div>
        <div v-else-if="pagedTickets.length > 0"
          class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCard v-for="ticket in pagedTickets" :key="ticket.id"
            :ticket="ticket" @click="openDetail(ticket)" class="cursor-pointer">
            <template #actions>
              <div class="flex flex-wrap items-center gap-2">
                <button @click.stop="updateStatus(ticket.id, 1)"
                  class="px-2 py-1 rounded bg-indigo-500/20 text-indigo-400 text-xs font-medium hover:bg-indigo-500/30 transition-colors">
                  Proceso
                </button>
                <button @click.stop="updateStatus(ticket.id, 4)"
                  class="px-2 py-1 rounded bg-amber-100 dark:bg-amber-900/40 text-amber-700 dark:text-amber-400 text-xs font-medium hover:bg-amber-500/30 transition-colors">
                  Espera
                </button>
                <button @click.stop="updateStatus(ticket.id, 2)"
                  class="px-2 py-1 rounded bg-emerald-500/20 text-emerald-400 text-xs font-medium hover:bg-emerald-500/30 transition-colors">
                  Resolver
                </button>
                <select @click.stop @change="assignOperator(ticket.id, $event)"
                  class="px-2 py-1 rounded text-xs border font-semibold cursor-pointer outline-none"
                  :class="'bg-purple-50 border-purple-200 text-purple-700 dark:bg-purple-500/20 dark:border-purple-500/30 dark:text-purple-300'">
                  <option value="">Asignar operador</option>
                  <option v-for="op in operators" :key="op.id" :value="op.id">{{ op.nombre }}</option>
                </select>
                <button @click.stop="removeTicket(ticket.id)"
                  class="px-2 py-1 rounded bg-rose-500/20 text-rose-400 text-xs font-medium hover:bg-rose-500/30 transition-colors">
                  Eliminar
                </button>
              </div>
            </template>
          </TicketCard>
        </div>
        <div v-else
          class="flex flex-col items-center justify-center py-32 border-2 border-dashed rounded-[3rem] transition-colors"
          :class="'border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900/50'">
          <InboxIcon class="w-10 h-10 mb-4" :class="'text-slate-300 dark:text-slate-700'" />
          <h3 class="text-xl font-black mb-2 uppercase italic">
            {{ ticketsStore.hayFiltrosActivos ? 'Sin resultados' : 'No hay tickets' }}
          </h3>
          <button v-if="ticketsStore.hayFiltrosActivos" @click="ticketsStore.clearFiltros()"
            class="mt-4 text-xs font-black uppercase tracking-widest text-blue-500 hover:text-blue-400 transition-colors">
            Limpiar filtros
          </button>
        </div>
        <div v-if="totalPages > 1"
          class="flex items-center justify-between pt-4 border-t"
          :class="'border-slate-100 dark:border-slate-800'">
          <p class="text-xs font-medium" :class="'text-slate-400 dark:text-slate-500'">
            Mostrando
            <span class="font-black" :class="'text-slate-900 dark:text-white'">
              {{ (currentPage - 1) * PAGE_SIZE + 1 }}–{{ Math.min(currentPage * PAGE_SIZE, allAdminTickets.length) }}
            </span>
            de
            <span class="font-black" :class="'text-slate-900 dark:text-white'">
              {{ allAdminTickets.length }}
            </span>
            tickets
          </p>
          <div class="flex items-center gap-1">
            <button @click="goToPage(currentPage - 1)" :disabled="currentPage === 1"
              class="p-2 rounded-lg border transition-all disabled:opacity-30 disabled:cursor-not-allowed"
              :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-500 dark:text-slate-400 hover:border-slate-300 dark:hover:border-slate-600'">
              <ChevronLeftIcon class="w-4 h-4" />
            </button>
            <template v-for="p in visiblePages" :key="p">
              <span v-if="p === '...'" class="px-2 text-xs" :class="'text-slate-400 dark:text-slate-600'">···</span>
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
              :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-500 dark:text-slate-400 hover:border-slate-300 dark:hover:border-slate-600'">
              <ChevronRightIcon class="w-4 h-4" />
            </button>
          </div>
          <p class="text-[10px] font-bold uppercase tracking-widest" :class="'text-slate-400 dark:text-slate-600'">
            Página {{ currentPage }} de {{ totalPages }}
          </p>
        </div>
      </section>

      <section v-else-if="activeTab === 'usuarios'" class="space-y-3">
        <div v-if="isLoadingUsers">
          <UserCardSkeleton v-for="n in 8" :key="n" />
        </div>
        <div v-else-if="filteredUsers.length > 0">
          <div class="flex justify-end mb-2">
            <button v-if="dirtyUsers.length > 0" @click="applyChanges"
              class="px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded-lg font-semibold transition-colors">
              Aplicar cambios
            </button>
          </div>
          <div v-for="user in filteredUsers" :key="user.id"
            class="rounded-2xl p-4 flex items-center justify-between border mb-2 transition-colors"
            :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-sm'">
            <div>
              <div class="font-semibold">{{ user.nombre }}</div>
              <div class="text-xs" :class="'text-slate-400 dark:text-slate-500'">{{ user.email }}</div>
            </div>
            <div class="flex items-center gap-2">
              <select v-model="user.rol" @change="changeRole(user)"
                class="border rounded-lg px-2 py-1 text-sm outline-none transition"
                :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white'">
                <option :value="0">Usuario</option>
                <option :value="1">Operador</option>
                <option :value="2">Administrador</option>
              </select>
              <button @click="deleteUser(user.id)"
                class="px-3 py-1.5 rounded-lg bg-rose-500/10 border border-rose-500/20 text-rose-400 text-xs font-semibold hover:bg-rose-500/20 transition-colors">
                Eliminar
              </button>
            </div>
          </div>
        </div>
        <div v-else class="flex flex-col items-center justify-center py-32 border-2 border-dashed rounded-[3rem]"
          :class="'border-slate-200 dark:border-slate-800'">
          <h3 class="text-xl font-black mb-2 uppercase italic">No se encontraron usuarios</h3>
        </div>
      </section>

      <section v-else-if="activeTab === 'metricas'" class="space-y-6">
        <div class="grid md:grid-cols-2 gap-6">
          <div class="rounded-2xl p-6 border transition-colors"
            :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-sm'">
            <h3 class="text-lg font-black mb-6">Distribución de Estados</h3>
            <div class="relative h-64 flex items-center justify-center">
              <canvas ref="donutChartRef"></canvas>
            </div>
          </div>
          <div class="rounded-2xl p-6 border transition-colors"
            :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-sm'">
            <h3 class="text-lg font-black mb-6">Tickets por Prioridad</h3>
            <div class="relative h-64">
              <canvas ref="barChartRef"></canvas>
            </div>
          </div>
        </div>
        <div class="rounded-2xl p-6 border transition-colors"
          :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-sm'">
          <h3 class="text-lg font-black mb-6">Indicadores Clave</h3>
          <div class="grid md:grid-cols-2 gap-3">
            <div v-for="kpi in kpis" :key="kpi.label"
              class="flex items-center justify-between p-3 rounded-lg"
              :class="'bg-slate-50 dark:bg-slate-800'">
              <span class="text-sm" :class="'text-slate-500 dark:text-slate-400'">{{ kpi.label }}</span>
              <span class="font-bold" :class="kpi.color">{{ kpi.value }}</span>
            </div>
          </div>
        </div>
      </section>

      <section v-else-if="activeTab === 'auditoria'" class="space-y-3 max-h-[600px] overflow-y-auto custom-scrollbar">
        <div v-if="auditLog.length > 0" class="space-y-3">
          <div v-for="entry in auditLog" :key="entry.id"
            class="rounded-xl p-4 border flex items-start gap-4 transition-colors"
            :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-sm'">
            <div :class="`w-2 h-2 rounded-full mt-2 flex-shrink-0 ${
              entry.type === 'delete' ? 'bg-rose-500' :
              entry.type === 'create' ? 'bg-emerald-500' :
              entry.type === 'update' ? 'bg-blue-500' : 'bg-slate-500'}`"></div>
            <div class="flex-1">
              <div class="text-sm font-semibold">{{ entry.message || entry.detalle }}</div>
              <div class="text-xs mt-1" :class="'text-slate-400 dark:text-slate-500'">
                {{ formatTime(entry.timestamp || entry.fecha) }}
              </div>
            </div>
          </div>
        </div>
        <div v-else class="text-center py-12 text-sm" :class="'text-slate-400 dark:text-slate-600'">
          No hay eventos de auditoría.
        </div>
      </section>

      <section v-else class="space-y-6">
        <div class="rounded-2xl p-6 border transition-colors"
          :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-sm'">
          <h3 class="text-lg font-black mb-6">Configuración Global</h3>
          <div class="space-y-4">
            <div v-for="field in configFields" :key="field.label" class="space-y-2">
              <label class="text-xs font-bold uppercase tracking-widest"
                :class="'text-slate-400 dark:text-slate-500'">{{ field.label }}</label>
              <input v-model="field.value" :type="field.type" :placeholder="field.placeholder"
                class="w-full border rounded-lg px-4 py-2 outline-none transition"
                :class="'bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white focus:border-blue-500 dark:placeholder-slate-600'" />
            </div>
            <button @click="saveBrandSettings"
              class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 rounded-lg transition-colors">
              Guardar Cambios
            </button>
          </div>
        </div>
        <DataExport :tickets="ticketsStore.tickets" :usuarios="users" :auditLog="auditLog" />
        <div class="rounded-2xl p-6 border transition-colors"
          :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 shadow-sm'">
          <h3 class="text-lg font-black mb-6">Acciones del Sistema</h3>
          <div class="grid md:grid-cols-2 gap-4">
            <button @click="clearAuditLog"
              class="px-4 py-3 rounded-lg text-sm font-semibold transition-colors border"
              :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 hover:bg-slate-100 dark:hover:bg-slate-700'">
              Limpiar Auditoría
            </button>
            <button @click="resetSystemConfirm"
              class="px-4 py-3 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 rounded-lg transition-colors text-sm font-semibold border border-rose-500/20">
              Resetear Sistema
            </button>
            <button @click="viewSystemLogs"
              class="px-4 py-3 rounded-lg text-sm font-semibold transition-colors border"
              :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 hover:bg-slate-100 dark:hover:bg-slate-700'">
              Ver Logs
            </button>
            <button @click="syncDatabase"
              class="px-4 py-3 rounded-lg text-sm font-semibold transition-colors border"
              :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 hover:bg-slate-100 dark:hover:bg-slate-700'">
              Sincronizar BD
            </button>
          </div>
        </div>
      </section>
    </main>

    <TicketDetailModal v-if="ticketsStore.selectedTicket"
      :ticket="ticketsStore.selectedTicket"
      @close="ticketsStore.clearSelection()" />

    <Teleport to="body">
      <div v-if="userToDelete" class="fixed inset-0 z-50 flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/60 backdrop-blur-sm" @click="userToDelete = null" />
        <div class="relative rounded-2xl p-6 w-full max-w-sm shadow-2xl border transition-colors"
          :class="'bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800'">
          <div class="flex flex-col items-center text-center gap-4">
            <div class="w-12 h-12 rounded-full bg-rose-500/10 flex items-center justify-center">
              <svg class="w-6 h-6 text-rose-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </div>
            <div>
              <h3 class="text-base font-black">¿Eliminar usuario?</h3>
              <p class="text-sm mt-1" :class="'text-slate-500 dark:text-slate-400'">Esta acción no se puede deshacer.</p>
              <p class="text-sm font-semibold mt-2 text-rose-400">{{ userToDelete?.nombre }}</p>
            </div>
            <div class="flex gap-3 w-full">
              <button @click="userToDelete = null"
                class="flex-1 px-4 py-2 rounded-xl text-sm font-semibold border transition-colors"
                :class="'bg-slate-100 dark:bg-slate-800 border-slate-200 dark:border-slate-700 hover:bg-slate-200 dark:hover:bg-slate-700 text-slate-700 dark:text-white'">
                Cancelar
              </button>
              <button @click="confirmDeleteUser"
                class="flex-1 px-4 py-2 rounded-xl text-sm font-semibold bg-rose-500 hover:bg-rose-600 text-white transition-colors">
                Eliminar
              </button>
            </div>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, onUnmounted, nextTick } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { useAuthStore, API_URL } from '../store/auth';
import { useTicketsStore } from '../store/tickets';
import { useNotificationStore } from '../store/notifications';
import { useSettingsStore } from '../store/settings';
import {
  SearchIcon, RefreshCcwIcon, InboxIcon,
  ChevronLeftIcon, ChevronRightIcon
} from 'lucide-vue-next';
import TicketDetailModal  from '../components/TicketDetailModal.vue';
import TicketCard         from '../components/TicketCard.vue';
import TicketCardSkeleton from '../components/TicketCardSkeleton.vue';
import UserCardSkeleton   from '../components/UserCardSkeleton.vue';
import DataExport         from '../components/DataExport.vue';
import Sidebar            from '../components/Sidebar.vue';
import TicketFilters      from '../components/TicketFilters.vue';

const router            = useRouter();
const authStore         = useAuthStore();
const ticketsStore      = useTicketsStore();
const notificationStore = useNotificationStore();
const settingsStore     = useSettingsStore();

const activeTab = ref('dashboard');
const tabs = [
  { key: 'dashboard', label: 'Dashboard' },
  { key: 'usuarios',  label: 'Usuarios' },
  { key: 'metricas',  label: 'Métricas' },
  { key: 'auditoria', label: 'Auditoría' },
  { key: 'config',    label: 'Configuración' },
];

const users         = ref([]);
const operators     = ref([]);
const originalRoles = ref({});
const auditLog      = ref([{ id: 1, message: 'Sistema inicializado.', type: 'system', timestamp: Date.now() }]);
const isLoadingTickets = ref(false);
const isLoadingUsers   = ref(false);
const ticketsSearch    = ref('');
const usersSearch      = ref('');
const currentPage = ref(1);
const PAGE_SIZE   = 12;

const donutChartRef = ref(null);
const barChartRef   = ref(null);
let donutChart      = null;
let barChart        = null;

const allAdminTickets = computed(() => {
  if (!ticketsSearch.value) return ticketsStore.tickets;
  const q = ticketsSearch.value.toLowerCase();
  return ticketsStore.tickets.filter(t =>
    t.titulo?.toLowerCase().includes(q) ||
    t.usuarioNombre?.toLowerCase().includes(q)
  );
});

const totalPages = computed(() => Math.ceil(allAdminTickets.value.length / PAGE_SIZE) || 1);
const pagedTickets = computed(() => {
  const start = (currentPage.value - 1) * PAGE_SIZE;
  return allAdminTickets.value.slice(start, start + PAGE_SIZE);
});
const visiblePages = computed(() => {
  const total = totalPages.value, current = currentPage.value;
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
  const pages = [1];
  if (current > 3) pages.push('...');
  for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) pages.push(i);
  if (current < total - 2) pages.push('...');
  pages.push(total);
  return pages;
});
const goToPage = (p) => {
  if (p < 1 || p > totalPages.value) return;
  currentPage.value = p;
};

watch(ticketsSearch, () => { currentPage.value = 1; });

const configFields = ref([
  { label: 'Nombre de Marca',      value: 'TicketSystem Enterprise', type: 'text',   placeholder: 'Nombre de marca' },
  { label: 'Email de Soporte',     value: '',                        type: 'email',  placeholder: 'support@example.com' },
  { label: 'SLA Por Defecto (hs)', value: '',                        type: 'number', placeholder: '24' },
]);

const realMetrics = ref({
  totalTickets: 0, ticketsPendientes: 0, ticketsEnProceso: 0,
  ticketsResueltos: 0, porcentajeSlaCumplido: 100,
  tiempoPromedioResolucion: '0h 0m', distribucionPorEstado: {}
});

const metrics = computed(() => [
  { label: 'Total',       value: realMetrics.value.totalTickets },
  { label: 'Pendientes',  value: realMetrics.value.ticketsPendientes },
  { label: 'En Progreso', value: realMetrics.value.ticketsEnProceso },
  { label: 'Resueltos',   value: realMetrics.value.ticketsResueltos },
  { label: 'SLA',         value: realMetrics.value.porcentajeSlaCumplido + '%' },
]);

const kpis = computed(() => [
  { label: 'Tiempo promedio resolución', value: realMetrics.value.tiempoPromedioResolucion, color: 'text-emerald-400' },
  { label: 'Resueltos hoy',              value: realMetrics.value.resueltosHoy ?? 0,        color: 'text-blue-400' },
  { label: 'Tasa de satisfacción',       value: '94%',                                       color: 'text-purple-400' },
  { label: 'Usuarios activos',           value: users.value.length,                          color: 'text-pink-400' },
]);

const filteredUsers = computed(() => {
  if (!usersSearch.value) return users.value;
  const q = usersSearch.value.toLowerCase();
  return users.value.filter(u => u.nombre?.toLowerCase().includes(q) || u.email?.toLowerCase().includes(q));
});

const dirtyUsers = computed(() => users.value.filter(u => u._dirty));

const isDark = computed(() => settingsStore.isDark);

const buildCharts = async () => {
  await nextTick();
  if (!donutChartRef.value || !barChartRef.value) return;

  const { Chart, ArcElement, DoughnutController, BarController, CategoryScale, LinearScale, BarElement, Tooltip, Legend } = await import('chart.js');
  Chart.register(ArcElement, DoughnutController, BarController, CategoryScale, LinearScale, BarElement, Tooltip, Legend);

  const dist   = realMetrics.value.distribucionPorEstado ?? {};
  const labels = Object.keys(dist);
  const values = Object.values(dist);

  const textColor   = isDark.value ? '#94a3b8' : '#64748b';
  const gridColor   = isDark.value ? 'rgba(148,163,184,0.1)' : 'rgba(0,0,0,0.06)';

  if (donutChart) donutChart.destroy();
  donutChart = new Chart(donutChartRef.value, {
    type: 'doughnut',
    data: {
      labels,
      datasets: [{
        data: values,
        backgroundColor: ['#3b82f6','#6366f1','#10b981','#f59e0b','#ef4444'],
        borderWidth: 0,
        hoverOffset: 6
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      cutout: '65%',
      plugins: {
        legend: {
          position: 'bottom',
          labels: { color: textColor, padding: 12, font: { size: 11, weight: 'bold' } }
        },
        tooltip: { callbacks: { label: (ctx) => ` ${ctx.label}: ${ctx.raw}` } }
      }
    }
  });

  const tickets = ticketsStore.tickets;
  const prioridades = ['Baja', 'Media', 'Alta', 'Critica'];
  const counts = prioridades.map(p =>
    tickets.filter(t => t.prioridad === p || t.prioridad === p.toLowerCase()).length
  );

  if (barChart) barChart.destroy();
  barChart = new Chart(barChartRef.value, {
    type: 'bar',
    data: {
      labels: prioridades,
      datasets: [{
        label: 'Tickets',
        data: counts,
        backgroundColor: ['#10b981','#3b82f6','#f59e0b','#ef4444'],
        borderRadius: 6,
        borderSkipped: false
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { display: false },
        tooltip: { callbacks: { label: (ctx) => ` ${ctx.raw} tickets` } }
      },
      scales: {
        x: { ticks: { color: textColor, font: { weight: 'bold' } }, grid: { display: false } },
        y: { ticks: { color: textColor }, grid: { color: gridColor }, beginAtZero: true }
      }
    }
  });
};

const fetchTickets = async () => {
  isLoadingTickets.value = true;
  try { await ticketsStore.fetchAll(); }
  catch { notificationStore.error('No se pudieron cargar los tickets.'); }
  finally { isLoadingTickets.value = false; }
};

const fetchUsers = async () => {
  isLoadingUsers.value = true;
  try {
    const res = await axios.get(`${API_URL}/usuarios`);
    const rolMap = { 'Usuario': 0, 'Operador': 1, 'Administrador': 2 };
    users.value = (res.data?.data || res.data || []).map(u => ({
      ...u,
      rol: typeof u.rol === 'string' ? (rolMap[u.rol] ?? 0) : u.rol
    }));
    originalRoles.value = {};
    users.value.forEach(u => { originalRoles.value[u.id] = u.rol; u._dirty = false; });
  } catch { notificationStore.error('No se pudieron cargar los usuarios.'); }
  finally { isLoadingUsers.value = false; }
};

const fetchMetrics = async () => {
  try {
    const res = await axios.get(`${API_URL}/metricas`);
    realMetrics.value = res.data?.data || realMetrics.value;
    if (activeTab.value === 'metricas') buildCharts();
  } catch (e) { console.error(e); }
};

const fetchAuditLogs = async () => {
  try {
    const res = await axios.get(`${API_URL}/auditoria`);
    auditLog.value = (res.data?.data || []).map(l => ({
      id:        l.id || l.Id,
      message:   l.detalle || l.Detalle,
      type:      (l.accion || 'update').toLowerCase(),
      timestamp: l.fecha || l.Fecha
    }));
  } catch (e) { console.error(e); }
};

const fetchOperators = async () => {
  try {
    const res = await axios.get(`${API_URL}/usuarios`);
    operators.value = (res.data?.data || res.data || []).filter(u =>
      u.rol === 1 || u.rol === 2 || u.rol === 'Operador' || u.rol === 'Administrador'
    );
  } catch (e) { console.error(e); }
};

const updateStatus = async (id, s) => {
  try {
    await ticketsStore.updateStatus(id, s);
    const labels = { 1: 'En Proceso', 2: 'Resuelto', 4: 'En Espera' };
    notificationStore.success(`Ticket → ${labels[s]}`);
    addAudit(`Ticket cambió a estado ${labels[s]}`, 'update');
  } catch { notificationStore.error('No se pudo cambiar el estado.'); }
};

const removeTicket   = async (id)    => { await ticketsStore.deleteTicket(id); addAudit(`Ticket ${id} eliminado`, 'delete'); };
const openDetail     = async (t)     => { ticketsStore.selectTicket(t); await ticketsStore.fetchComments(t.id); };
const assignOperator = async (id, e) => {
  const opId = e.target.value; if (!opId) return;
  try { await ticketsStore.assignOperator(id, opId); notificationStore.success('Operador asignado'); await fetchTickets(); }
  catch { notificationStore.error('Error al asignar operador'); }
};

const changeRole = (u) => { u._dirty = u.rol !== (originalRoles.value[u.id] ?? u.rol); };

const userToDelete = ref(null);
const deleteUser = (id) => { userToDelete.value = users.value.find(u => u.id === id); };
const confirmDeleteUser = async () => {
  if (!userToDelete.value) return;
  const id = userToDelete.value.id;
  userToDelete.value = null;
  try {
    await axios.delete(`${API_URL}/usuarios/${id}`);
    users.value = users.value.filter(u => u.id !== id);
    notificationStore.success('Usuario eliminado.');
  } catch { notificationStore.error('No se pudo eliminar.'); }
};

const applyChanges = async () => {
  try {
    await axios.put(`${API_URL}/usuarios/roles`, dirtyUsers.value.map(u => ({ id: u.id, rol: u.rol })));
    dirtyUsers.value.forEach(u => { originalRoles.value[u.id] = u.rol; u._dirty = false; });
    notificationStore.success('Cambios aplicados.');
  } catch { notificationStore.error('No se pudieron aplicar los cambios.'); }
};

const saveBrandSettings  = async () => { notificationStore.info('Guardando...'); await new Promise(r => setTimeout(r, 1000)); notificationStore.success('Configuración guardada.'); };
const clearAuditLog      = () => { if (confirm('¿Limpiar auditoría?')) { auditLog.value = []; notificationStore.success('Auditoría limpiada.'); } };
const resetSystemConfirm = () => { if (prompt('Escribe "CONFIRMAR":') === 'CONFIRMAR') { ticketsStore.tickets = []; users.value = []; auditLog.value = []; notificationStore.success('Sistema reseteado.'); } };
const viewSystemLogs     = () => { console.log('Logs:', { tickets: ticketsStore.tickets.length, users: users.value.length }); notificationStore.info('Logs en consola.'); };
const syncDatabase       = () => { notificationStore.info('Sincronizando...'); setTimeout(() => notificationStore.success('BD sincronizada.'), 1500); };

const formatTime = (ts) => {
  const str = ts?.toString() ?? '';
  const normalized = str.endsWith('Z') || str.includes('+') ? str : str + 'Z';
  return new Date(normalized).toLocaleString('es-AR', {
    timeZone: 'America/Argentina/Buenos_Aires',
    hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit'
  });
};

const addAudit = (msg, type = 'update') => {
  auditLog.value.unshift({ id: Date.now(), message: msg, type, timestamp: Date.now() });
  if (auditLog.value.length > 50) auditLog.value.pop();
};

onMounted(async () => {
  if (!authStore.isAdmin) { router.replace('/dashboard'); return; }
  await Promise.all([fetchTickets(), fetchUsers(), fetchOperators(), fetchMetrics(), fetchAuditLogs()]);
  const onKey = (e) => {
    if (['INPUT', 'TEXTAREA', 'SELECT'].includes(e.target.tagName)) return;
    if (e.key === '1') activeTab.value = 'dashboard';
    if (e.key === '2') activeTab.value = 'usuarios';
    if (e.key === '3') activeTab.value = 'metricas';
    if (e.key === '4') activeTab.value = 'auditoria';
    if (e.key === '5') activeTab.value = 'config';
    if (e.key === 'ArrowRight') goToPage(currentPage.value + 1);
    if (e.key === 'ArrowLeft')  goToPage(currentPage.value - 1);
  };
  window.addEventListener('keydown', onKey);
  onUnmounted(() => {
    window.removeEventListener('keydown', onKey);
    if (donutChart) donutChart.destroy();
    if (barChart)   barChart.destroy();
  });
});

watch(activeTab, (v) => {
  currentPage.value = 1;
  if (v === 'dashboard') fetchTickets();
  if (v === 'usuarios')  fetchUsers();
  if (v === 'metricas')  { fetchMetrics(); nextTick(buildCharts); }
  if (v === 'auditoria') fetchAuditLogs();
});

watch(isDark, () => {
  if (activeTab.value === 'metricas') nextTick(buildCharts);
});
</script>