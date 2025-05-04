<script setup lang="ts">
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted } from 'vue';

const groupStore = useGroupStore();
const { groups, current, currentGroup } = storeToRefs(groupStore)

onMounted(()=>{
    groupStore.getGroupsForOrganization(1);
});
</script>

<template>
    <select v-model="current">
        <option v-for="group in groups" :value="group.id">{{group.name}}</option>
    </select>
    <div v-show="current>0">
        <h3>Requirements</h3>
        <div v-for="requirement in currentGroup?.requirements">
            <p>
                Activity: {{requirement.activity.title}}
            </p>
            <p>
                Duration: {{requirement.duration}}
            </p>
            <p>
                Hall Type: {{requirement.halltype}}
            </p>
            <p>
                Per week: {{requirement.timesPerWeek}}
            </p>
        </div>
    </div>
</template>