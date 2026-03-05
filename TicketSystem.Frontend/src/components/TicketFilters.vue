<template>
  <div class="rounded-2xl border transition-colors"
    :class="settingsStore.isDark ? 'bg-slate-900 border-slate-800' : 'bg-white border-slate-200'">

    <!-- Header -->
    <div class="flex items-center justify-between px-5 py-3 border-b cursor-pointer"
      :class="settingsStore.isDark ? 'border-slate-800' : 'border-slate-100'"
      @click="expanded = !expanded">
      <div class="flex items-center gap-3">
        <SlidersHorizontalIcon class="w-4 h-4 text-blue-500" />
        <span class="text-xs font-black uppercase tracking-widest">Filtros</span>
        <span v-if="ticketsStore.hayFiltrosActivos"
          class="w-2 h-2 rounded-full bg-blue-500"></span>
      </div>
      <div class="flex items-center gap-3">
        <button v-if="ticketsStore.hayFiltrosActivos"
          @click.stop="ticketsStore.clearFiltros()"
          class="text-[10px] font-black uppercase tracking-widest text-rose-400 hover:text-rose-300 transition-colors">
          Limpiar
        </button>
        <ChevronDownIcon class="w-4 h-4 transition-transform"
          :class="[expanded ? 'rotate-180' : '', settingsStore.isDark ? 'text-slate-500' : 'text-slate-400']" />
      </div>
    </div>

    <!-- Filtros -->
    <div v-if="expanded" class="p-5 grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-4">

  <!-- Búsqueda (2 cols) -->
<div class="col-span-2 relative">
  <label class="block text-[9px] font-black uppercase tracking-widest mb-1.5 invisible">
    Buscar
  </label>
  <div class="relative">
    <SearchIcon class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2"
      :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'" />
    <input v-model="localFiltros.titulo" @input="debouncedApply"
      placeholder="Buscar por título..."
      class="w-full border pl-9 pr-4 py-2.5 rounded-xl text-sm outline-none transition"
      :class="settingsStore.isDark
        ? 'bg-slate-800 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
        : 'bg-slate-50 border-slate-200 text-slate-900 placeholder-slate-400 focus:border-blue-500'" />
  </div>
</div>

  <!-- Estado -->
  <div>
    <label class="block text-[9px] font-black uppercase tracking-widest mb-1.5"
      :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">Estado</label>
    <select v-model="localFiltros.estado" @change="applyFiltros"
      class="w-full border px-3 py-2.5 rounded-xl text-sm outline-none transition"
      :class="settingsStore.isDark
        ? 'bg-slate-800 border-slate-700 text-white focus:border-blue-500'
        : 'bg-slate-50 border-slate-200 text-slate-900 focus:border-blue-500'">
      <option :value="null">Todos</option>
      <option value="0">Pendiente</option>
      <option value="1">En Proceso</option>
      <option value="2">Resuelto</option>
      <option value="3">Cerrado</option>
      <option value="4">Esperando Usuario</option>
    </select>
  </div>

  <!-- Prioridad -->
  <div>
    <label class="block text-[9px] font-black uppercase tracking-widest mb-1.5"
      :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">Prioridad</label>
    <select v-model="localFiltros.prioridad" @change="applyFiltros"
      class="w-full border px-3 py-2.5 rounded-xl text-sm outline-none transition"
      :class="settingsStore.isDark
        ? 'bg-slate-800 border-slate-700 text-white focus:border-blue-500'
        : 'bg-slate-50 border-slate-200 text-slate-900 focus:border-blue-500'">
      <option :value="null">Todas</option>
      <option value="0">Baja</option>
      <option value="1">Media</option>
      <option value="2">Alta</option>
    </select>
  </div>

  <!-- Fecha Desde -->
  <div>
    <label class="block text-[9px] font-black uppercase tracking-widest mb-1.5"
      :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">Desde</label>
    <input v-model="localFiltros.fechaDesde" @change="applyFiltros" type="date"
      class="w-full border px-3 py-2.5 rounded-xl text-sm outline-none transition"
      :class="settingsStore.isDark
        ? 'bg-slate-800 border-slate-700 text-white focus:border-blue-500'
        : 'bg-slate-50 border-slate-200 text-slate-900 focus:border-blue-500'" />
  </div>

  <!-- Fecha Hasta -->
  <div>
    <label class="block text-[9px] font-black uppercase tracking-widest mb-1.5"
      :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">Hasta</label>
    <input v-model="localFiltros.fechaHasta" @change="applyFiltros" type="date"
      class="w-full border px-3 py-2.5 rounded-xl text-sm outline-none transition"
      :class="settingsStore.isDark
        ? 'bg-slate-800 border-slate-700 text-white focus:border-blue-500'
        : 'bg-slate-50 border-slate-200 text-slate-900 focus:border-blue-500'" />
  </div>

  <!-- Ordenar por + dirección -->
  <div class="flex gap-2 items-end">
    <div class="flex-1">
      <label class="block text-[9px] font-black uppercase tracking-widest mb-1.5"
        :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">Ordenar por</label>
      <select v-model="localFiltros.sortBy" @change="applyFiltros"
        class="w-full border px-3 py-2.5 rounded-xl text-sm outline-none transition"
        :class="settingsStore.isDark
          ? 'bg-slate-800 border-slate-700 text-white focus:border-blue-500'
          : 'bg-slate-50 border-slate-200 text-slate-900 focus:border-blue-500'">
        <option value="fechacreacion">Fecha</option>
        <option value="titulo">Título</option>
        <option value="estado">Estado</option>
        <option value="prioridad">Prioridad</option>
      </select>
    </div>
    <button @click="toggleDescending"
      class="p-2.5 rounded-xl border transition-all mb-0.5"
      :class="settingsStore.isDark
        ? 'bg-slate-800 border-slate-700 text-slate-400 hover:border-blue-500'
        : 'bg-slate-50 border-slate-200 text-slate-500 hover:border-blue-400'">
      <ArrowDownIcon v-if="localFiltros.descending" class="w-4 h-4" />
      <ArrowUpIcon   v-else                         class="w-4 h-4" />
    </button>
  </div>

</div>

  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import {
  SlidersHorizontalIcon, ChevronDownIcon, SearchIcon,
  ArrowUpIcon, ArrowDownIcon
} from 'lucide-vue-next';
import { useTicketsStore } from '../store/tickets';
import { useSettingsStore } from '../store/settings';

const ticketsStore  = useTicketsStore();
const settingsStore = useSettingsStore();

const expanded = ref(true);

const localFiltros = reactive({
  titulo:     '',
  estado:     null,
  prioridad:  null,
  fechaDesde: null,
  fechaHasta: null,
  sortBy:     'fechacreacion',
  descending: true,
});

const applyFiltros = () => {
  ticketsStore.applyFiltros({ ...localFiltros });
};

// Debounce para búsqueda por texto
let debounceTimer = null;
const debouncedApply = () => {
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(applyFiltros, 400);
};

const toggleDescending = () => {
  localFiltros.descending = !localFiltros.descending;
  applyFiltros();
};
</script>