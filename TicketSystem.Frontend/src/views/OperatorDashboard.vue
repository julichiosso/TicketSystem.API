<template>
  <div class="min-h-screen flex transition-colors duration-300"
    :class="settingsStore.isDark ? 'bg-slate-950 text-white' : 'bg-slate-50 text-slate-900'">
    <Sidebar />

    <main class="flex-1 p-8 space-y-6 overflow-y-auto h-screen custom-scrollbar">

      <!-- Header -->
      <header class="flex flex-col gap-6 md:flex-row md:items-center justify-between">
        <div>
          <h2 class="text-xl font-semibold tracking-tight">Panel Superior</h2>
          <p class="text-[10px] font-medium uppercase tracking-[0.1em]"
            :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
            Operador de Resoluciones
          </p>
        </div>

        <div class="flex flex-wrap items-center gap-4">
          <!-- Tabs -->
          <div class="p-1 rounded-xl border flex gap-1"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-slate-100 border-slate-200'">
            <button v-for="tab in tabs" :key="tab.key" @click="activeTab = tab.key"
              class="px-5 py-2 rounded-lg text-[10px] font-bold uppercase tracking-widest transition-all duration-200"
              :class="activeTab === tab.key
                ? settingsStore.isDark ? 'bg-slate-800 text-blue-400 shadow-sm' : 'bg-white text-blue-600 shadow-sm'
                : settingsStore.isDark ? 'text-slate-500 hover:text-slate-300' : 'text-slate-400 hover:text-slate-600'">
              {{ tab.label }}
            </button>
          </div>

          <button @click="fetchTickets"
            class="p-2.5 rounded-xl transition-all active:rotate-180 duration-700"
            :class="settingsStore.isDark ? 'hover:bg-slate-800 text-slate-400' : 'hover:bg-slate-200 text-slate-500'">
            <RefreshCcwIcon class="w-5 h-5" />
          </button>
        </div>
      </header>

      <!-- Tickets Tab -->
      <section v-if="activeTab === 'tickets'" class="space-y-4">
        <!-- Métricas rápidas -->
        <div class="grid md:grid-cols-4 gap-3">
          <div v-for="metric in metrics" :key="metric.label"
            class="rounded-xl p-4 border transition-all hover:scale-[1.02]"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200 shadow-sm'">
            <div class="text-xs mb-1"
              :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">{{ metric.label }}</div>
            <div class="text-2xl font-bold">{{ metric.value }}</div>
          </div>
        </div>

        <div v-if="isLoadingTickets" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCardSkeleton v-for="n in 6" :key="n" />
        </div>

        <div v-else-if="assignedTickets.length > 0"
          class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
          <TicketCard v-for="ticket in assignedTickets" :key="ticket.id"
            :ticket="ticket" @click="openDetail(ticket)" class="cursor-pointer">
            <template #actions>
              <div class="flex items-center gap-2">
                <button @click.stop="updateStatus(ticket.id, 1)"
                  class="px-2 py-1 rounded bg-indigo-500/20 text-indigo-400 text-xs font-medium hover:bg-indigo-500/30 transition-colors">
                  Proceso
                </button>
                <button @click.stop="updateStatus(ticket.id, 4)"
                  class="px-2 py-1 rounded bg-amber-500/20 text-amber-400 text-xs font-medium hover:bg-amber-500/30 transition-colors">
                  Espera
                </button>
                <button @click.stop="updateStatus(ticket.id, 2)"
                  class="px-2 py-1 rounded bg-emerald-500/20 text-emerald-400 text-xs font-medium hover:bg-emerald-500/30 transition-colors">
                  Resolver
                </button>
              </div>
            </template>
          </TicketCard>
        </div>

        <div v-else
          class="flex flex-col items-center justify-center py-32 border-2 border-dashed rounded-[3rem] transition-colors"
          :class="settingsStore.isDark ? 'border-slate-800 bg-slate-900/50' : 'border-slate-200 bg-white'">
          <div class="w-24 h-24 rounded-full flex items-center justify-center mb-8 border transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-800 border-slate-700' : 'bg-white border-slate-200'">
            <InboxIcon class="w-10 h-10 transition-colors"
              :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-300'" />
          </div>
          <h3 class="text-xl font-black mb-2 uppercase italic tracking-tight">Sin tickets asignados</h3>
          <p class="text-sm max-w-sm text-center font-medium leading-relaxed"
            :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
            Aún no tenés tickets asignados. El administrador los asignará pronto.
          </p>
        </div>
      </section>

      <!-- Stats Tab -->
      <section v-else-if="activeTab === 'stats'" class="space-y-6">
        <div class="grid md:grid-cols-2 gap-6">
          <div class="rounded-2xl p-6 border transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200 shadow-sm'">
            <h3 class="text-lg font-black mb-6">Mis Estadísticas</h3>
            <div class="space-y-3">
              <div v-for="stat in myStats" :key="stat.label"
                class="flex items-center justify-between p-3 rounded-lg"
                :class="settingsStore.isDark ? 'bg-slate-800' : 'bg-slate-50'">
                <span class="text-sm" :class="settingsStore.isDark ? 'text-slate-400' : 'text-slate-500'">
                  {{ stat.label }}
                </span>
                <span class="font-bold" :class="stat.color">{{ stat.value }}</span>
              </div>
            </div>
          </div>

          <div class="rounded-2xl p-6 border transition-colors"
            :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200 shadow-sm'">
            <h3 class="text-lg font-black mb-6">Distribución de Estados</h3>
            <div class="space-y-4">
              <div v-for="(value, label) in stateDistribution" :key="label" class="space-y-2">
                <div class="flex items-center justify-between">
                  <span class="text-sm" :class="settingsStore.isDark ? 'text-slate-400' : 'text-slate-500'">{{ label }}</span>
                  <span class="text-sm font-bold text-blue-400">{{ value }}</span>
                </div>
                <div class="w-full rounded-full h-2"
                  :class="settingsStore.isDark ? 'bg-slate-800' : 'bg-slate-100'">
                  <div class="bg-gradient-to-r from-blue-600 to-indigo-600 h-2 rounded-full transition-all duration-500"
                    :style="{ width: (value / (metrics[0]?.value || 1) * 100) + '%' }"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>

    <TicketDetailModal v-if="ticketsStore.selectedTicket"
      :ticket="ticketsStore.selectedTicket"
      @close="ticketsStore.clearSelection()" />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { useAuthStore, API_URL } from '../store/auth';
import { useTicketsStore } from '../store/tickets';
import { useSettingsStore } from '../store/settings';
import { RefreshCcwIcon, InboxIcon } from 'lucide-vue-next';
import TicketDetailModal from '../components/TicketDetailModal.vue';
import TicketCard from '../components/TicketCard.vue';
import TicketCardSkeleton from '../components/TicketCardSkeleton.vue';
import Sidebar from '../components/Sidebar.vue';

const router = useRouter();
const authStore = useAuthStore();
const ticketsStore = useTicketsStore();
const settingsStore = useSettingsStore();

const activeTab = ref('tickets');
const tabs = [
  { key: 'tickets', label: 'Mis Tickets' },
  { key: 'stats',   label: 'Estadísticas' },
];

const assignedTickets = ref([]);
const isLoadingTickets = ref(false);

const metrics = computed(() => {
  const total      = assignedTickets.value.length;
  const inProgress = assignedTickets.value.filter(t => t.estado === 1 || t.estado === 'EnProceso').length;
  const resolved   = assignedTickets.value.filter(t => t.estado === 2 || t.estado === 'Resuelto').length;
  const waiting    = assignedTickets.value.filter(t => t.estado === 4 || t.estado === 'EsperandoUsuario').length;
  return [
    { label: 'Total',       value: total },
    { label: 'En Progreso', value: inProgress },
    { label: 'Esperando',   value: waiting },
    { label: 'Resueltos',   value: resolved },
  ];
});

const resolutionRate = computed(() => {
  const total    = metrics.value[0].value;
  const resolved = metrics.value[3].value;
  return total > 0 ? Math.round((resolved / total) * 100) : 0;
});

const myStats = computed(() => [
  { label: 'Tickets totales',    value: metrics.value[0].value, color: 'text-blue-400' },
  { label: 'En progreso',        value: metrics.value[1].value, color: 'text-indigo-400' },
  { label: 'Resueltos',          value: metrics.value[3].value, color: 'text-emerald-400' },
  { label: 'Tasa de resolución', value: resolutionRate.value + '%', color: 'text-purple-400' },
]);

const stateDistribution = computed(() => ({
  'En Progreso':       metrics.value[1].value,
  'Esperando Usuario': metrics.value[2].value,
  'Resueltos':         metrics.value[3].value,
}));

const fetchTickets = async () => {
  isLoadingTickets.value = true;
  try {
    const res = await axios.get(`${API_URL}/tickets/operador/mis-tickets`);
    assignedTickets.value = res.data?.data || [];
  } catch (e) {
    console.error('Error fetching operator tickets:', e);
  } finally {
    isLoadingTickets.value = false;
  }
};

const updateStatus = async (ticketId, status) => {
  try {
    await axios.patch(`${API_URL}/tickets/${ticketId}/estado`, status);
    await fetchTickets();
  } catch (e) {
    console.error('Error updating status:', e);
  }
};

const openDetail = async (ticket) => {
  ticketsStore.selectTicket(ticket);
  await ticketsStore.fetchComments(ticket.id);
};

onMounted(() => {
  if (!authStore.isOperador) { router.push('/'); return; }
  fetchTickets();
});
</script>