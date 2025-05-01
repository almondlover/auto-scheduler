<script setup lang="ts">
import type { Group } from '@/classes/group';

const memberGroups:Group[]=[
    {
        id: 0,
        organizationId: 0,
        name: 'ISN',
        description: 'test description',
        subGroups: [{
            id: 0,
            organizationId: 0,
            name: '77',
            description: undefined,
            subGroups: [],
            requirements: []
        }],
        requirements: [{
            id: 0,
            duration: 0,
            hallsize: undefined,
            halltype: undefined,
            timesPerWeek: undefined,
            activity: {
                id: 0,
                organizationId: 0,
                title: 'test',
                description: ''
            },
            group: {
                id: 0,
                organizationId: 0,
                name: '',
                description: undefined,
                subGroups: [],
                requirements: []
            }
        }]
    },
    {
        id: 0,
        organizationId: 0,
        name: 'ISN2025',
        description: undefined,
        subGroups: [],
        requirements: []
    }
];
</script>

<template>
    <ul>
        <li v-for="group in memberGroups">
            <h3>
                {{group.name}}
            </h3>
            <div >
                SubGroups
                <RouterLink :to="`/groups/${group.id}`" v-for="subGroup in group.subGroups">{{subGroup.name}}</RouterLink>
            </div>
            <div >
                <!-- should maybe only be seen on the individual page? put description as placeholder-->
                Activity Scheduling Requirements
                <div v-for="requirement in group.requirements">
                    {{requirement.activity.description}}
                    <RouterLink :to="`/activities/${requirement.activity.id}`">{{requirement.activity.title}}</RouterLink>
                </div>
            </div>
            <RouterLink :to="`/groups/${group.id}`">Page</RouterLink>
            <RouterLink :to="`/timesheets/create`">Create timesheet</RouterLink>
        </li>
    </ul>
</template>