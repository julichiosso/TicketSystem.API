import { ref, onMounted, onUnmounted } from 'vue';

export function useInfiniteScroll(callback) {
  const isLoading = ref(false);
  const hasMore   = ref(true);

  const handleScroll = async () => {
    if (isLoading.value || !hasMore.value) return;

    const scrollTop    = window.scrollY || document.documentElement.scrollTop;
    const windowHeight = window.innerHeight;
    const docHeight    = document.documentElement.scrollHeight;

    if (scrollTop + windowHeight >= docHeight - 300) {
      isLoading.value = true;
      try {
        const result = await callback();
        if (result === false) hasMore.value = false;
      } finally {
        isLoading.value = false;
      }
    }
  };

  // También chequear si el contenido no llena la pantalla
  const checkIfNeedsMore = async () => {
    if (!hasMore.value || isLoading.value) return;
    if (document.documentElement.scrollHeight <= window.innerHeight) {
      isLoading.value = true;
      try {
        const result = await callback();
        if (result === false) hasMore.value = false;
        else setTimeout(checkIfNeedsMore, 100);
      } finally {
        isLoading.value = false;
      }
    }
  };

  onMounted(() => {
    window.addEventListener('scroll', handleScroll);
    setTimeout(checkIfNeedsMore, 500);
  });

  onUnmounted(() => {
    window.removeEventListener('scroll', handleScroll);
  });

  return { isLoading, hasMore };
}