/**
 * script.js
 * 
 * This script manages the interactive radar chart for user ratings on various aspects
 * such as Quantity, Quality, Variety, and Experience. It updates the chart in real-time
 * as users adjust their ratings via input sliders and handles form submission by
 * transferring the displayed values to hidden input fields.
 */
document.addEventListener('DOMContentLoaded', function () {

    /**
     * Initializes the radar chart with default values.
     */
    const ctx = document.getElementById('radar-chart').getContext('2d');
    const chart = new Chart(ctx, {
        type: 'radar',
        data: {
            labels: ['Quantity', 'Quality', 'Variety', 'Experience'],
            datasets: [{
                label: 'Your Rating',
                data: [3, 3, 3, 3],
                backgroundColor: 'rgba(76, 175, 80, 0.2)',
                borderColor: '#4CAF50',
                borderWidth: 2,
                pointBackgroundColor: '#4CAF50',
                pointRadius: 5
            }]
        },
        options: {
            scales: {
                r: {
                    angleLines: {
                        color: '#ECEFF1'
                    },
                    grid: {
                        color: '#ECEFF1'
                    },
                    pointLabels: {
                        font: {
                            size: 14,
                            weight: 'bold'
                        },
                        color: '#263238'
                    },
                    ticks: {
                        backdropColor: 'transparent',
                        color: '#263238',
                        stepSize: 1,
                        min: 0,
                        max: 5
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });

    /**
     *   Updates the rating value display and the radar chart when the input changes.
     * @param {*} inputId - The ID of the input element.
     * @param {*} valueId - The ID of the element displaying the value.
     */
    const updateRating = (inputId, valueId) => {
        const input = document.getElementById(inputId);
        const value = document.getElementById(valueId);

        input.addEventListener('input', function () {
            value.textContent = this.value;
            updateChart();
        });
    };

    /**
     * Updates the radar chart with the current rating values.
     */
    const updateChart = () => {
        chart.data.datasets[0].data = [
            parseInt(document.getElementById('quantity-value').textContent),
            parseInt(document.getElementById('quality-value').textContent),
            parseInt(document.getElementById('variety-value').textContent),
            parseInt(document.getElementById('experience-value').textContent)
        ];
        chart.update();
    };

    /**
     * Initialize all rating inputs and their corresponding display values.
     */
    updateRating('quantity', 'quantity-value');
    updateRating('quality', 'quality-value');
    updateRating('variety', 'variety-value');
    updateRating('experience', 'experience-value');

    /**
     * Handles the form submission by transferring the displayed rating values
     * to hidden input fields before submitting the form.
     */
    document.getElementById('submit-rating').addEventListener('click', function (e) {
        e.preventDefault();

        document.getElementById('quantity-input').value =
            document.getElementById('quantity-value').textContent;

        document.getElementById('quality-input').value =
            document.getElementById('quality-value').textContent;

        document.getElementById('variety-input').value =
            document.getElementById('variety-value').textContent;

        document.getElementById('experience-input').value =
            document.getElementById('experience-value').textContent;

        document.getElementById('comments-input').value =
            document.getElementById('comments').value;

        this.closest('form').submit();
    });

});